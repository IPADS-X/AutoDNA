"""Database configuration and models"""

from sqlalchemy import create_engine, Column, Integer, String, Float, DateTime, Text, ForeignKey, Enum as SQLEnum
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker, relationship, Session
from sqlalchemy.sql import func
from datetime import datetime
import json
import enum
from typing import Optional, List, Dict, Any

# Database setup - creates a local SQLite file for persistence
DATABASE_URL = "sqlite:///./reagent_manager.db"
engine = create_engine(DATABASE_URL, connect_args={"check_same_thread": False})
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

# ==================== Enums ====================

class Area(enum.IntEnum):
    """Enumeration of different lab areas"""
    CASEWORK = 1
    REPOSITORY = 2
    COLD_STORAGE = 3
    INCUBATOR = 4
    DISPOSAL = 5
    UNKNOWN = 0

class TubeStatus(enum.Enum):
    """Tube status enumeration"""
    AVAILABLE = "available"
    IN_USE = "in_use"
    DEPLETED = "depleted"

class TubeType(enum.IntEnum):
    """Tube capacity types"""
    P200 = 200
    P1000 = 1000
    P15K = 15000
    P50K = 50000

# ==================== Database Models ====================

class ReagentDB(Base):
    """Reagent database model"""
    __tablename__ = "reagents"
    
    # ID is auto-incremented by the database
    id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    name = Column(String, index=True, nullable=False, unique=True)  # Make name unique
    category = Column(String, nullable=False)
    form = Column(String, nullable=False)
    aliases = Column(Text, default="[]")  # JSON array
    properties = Column(Text, default="{}")  # JSON object
    components = Column(Text, default="[]")  # JSON array
    documentation = Column(Text, default="[]")  # JSON array
    notes = Column(Text, nullable=True)
    created_at = Column(DateTime, default=func.now())
    updated_at = Column(DateTime, default=func.now(), onupdate=func.now())
    
    # Relationship to tubes
    tubes = relationship("TubeDB", back_populates="reagent", cascade="all, delete-orphan")
    
    def to_dict(self, include_time: bool = False) -> Dict[str, Any]:
        """Convert to dictionary, optionally excluding timestamps."""
        data = {
            "id": self.id,
            "name": self.name,
            "category": self.category,
            "form": self.form,
            "aliases": json.loads(self.aliases) if self.aliases else [],
            "properties": json.loads(self.properties) if self.properties else {},
            "components": json.loads(self.components) if self.components else [],
            "documentation": json.loads(self.documentation) if self.documentation else [],
            "notes": self.notes
        }
        if include_time:
            data["created_at"] = self.created_at.isoformat() if self.created_at else None
            data["updated_at"] = self.updated_at.isoformat() if self.updated_at else None
        return data

class TubeDB(Base):
    """Tube database model"""
    __tablename__ = "tubes"
    
    # ID is auto-incremented by the database
    id = Column(Integer, primary_key=True, index=True, autoincrement=True)
    reagent_id = Column(Integer, ForeignKey("reagents.id"), nullable=True, index=True)
    capacity = Column(Integer, nullable=False)
    volume = Column(Float, default=0.0)
    label = Column(String, nullable=False)
    status = Column(String, default="available")
    location_x = Column(Integer, default=0)
    location_y = Column(Integer, default=0)
    location_area = Column(Integer, default=0)
    is_casework = Column(Integer, default=1)  # 1 for casework, 0 for repository
    created_at = Column(DateTime, default=func.now())
    updated_at = Column(DateTime, default=func.now(), onupdate=func.now())
    
    # Relationship to reagent
    reagent = relationship("ReagentDB", back_populates="tubes")
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary"""
        return {
            "id": self.id,
            "reagent_id": self.reagent_id,
            "capacity": self.capacity,
            "volume": self.volume,
            "label": self.label,
            "status": self.status,
            "location": {
                "x": self.location_x,
                "y": self.location_y,
                "area": Area(self.location_area).name
            },
            "is_casework": bool(self.is_casework),
            "created_at": self.created_at.isoformat() if self.created_at else None,
            "updated_at": self.updated_at.isoformat() if self.updated_at else None
        }

# Create tables
Base.metadata.create_all(bind=engine)

# ==================== Database Operations ====================

class DatabaseManager:
    """Manager for database operations"""
    
    @staticmethod
    def get_db():
        """Get database session"""
        db = SessionLocal()
        try:
            yield db
        finally:
            db.close()
    
    @staticmethod
    def create_reagent(db: Session, reagent_data: Dict[str, Any]) -> ReagentDB:
        """Create a new reagent in database - ID is auto-generated"""
        # Remove id if provided by user - let database handle it
        reagent_data = reagent_data.copy()
        reagent_data.pop('id', None)  # Remove id if it exists
        
        # Check if reagent with same name already exists
        existing = db.query(ReagentDB).filter(ReagentDB.name == reagent_data["name"]).first()
        if existing:
            raise ValueError(f"Reagent with name '{reagent_data['name']}' already exists (ID: {existing.id})")
        
        reagent = ReagentDB(
            name=reagent_data["name"],
            category=reagent_data["category"],
            form=reagent_data["form"],
            aliases=json.dumps(reagent_data.get("aliases", [])),
            properties=json.dumps(reagent_data.get("properties", {})),
            components=json.dumps(reagent_data.get("components", [])),
            documentation=json.dumps(reagent_data.get("documentation", [])),
            notes=reagent_data.get("notes")
        )
        db.add(reagent)
        db.commit()
        db.refresh(reagent)  # This will load the auto-generated ID
        return reagent
    
    @staticmethod
    def create_reagent_from_json_schema(db: Session, json_data: Dict[str, Any]) -> ReagentDB:
        """Create reagent from the original JSON schema format - handles legacy ID field"""
        # Convert the original schema to our format, ignoring any provided ID
        reagent_data = {
            "name": json_data["name"],
            "category": json_data.get("category", "Unknown"),
            "form": json_data.get("form", "Unknown"),
            "aliases": json_data.get("aliases", []),
            "properties": json_data.get("properties", {}),
            "components": json_data.get("components", []),
            "documentation": json_data.get("documentation", []),
            "notes": json_data.get("notes")
        }
        # Don't include the ID - let database generate it
        return DatabaseManager.create_reagent(db, reagent_data)
    
    @staticmethod
    def get_reagent(db: Session, reagent_id: int) -> Optional[ReagentDB]:
        """Get reagent by ID"""
        return db.query(ReagentDB).filter(ReagentDB.id == reagent_id).first()
    
    @staticmethod
    def get_reagent_by_name(db: Session, name: str) -> Optional[ReagentDB]:
        """Get reagent by name"""
        return db.query(ReagentDB).filter(ReagentDB.name == name).first()
    
    @staticmethod
    def get_all_reagents(db: Session, skip: int = 0, limit: int = 100) -> List[ReagentDB]:
        """Get all reagents with pagination"""
        return db.query(ReagentDB).offset(skip).limit(limit).all()
    
    @staticmethod
    def update_reagent(db: Session, reagent_id: int, update_data: Dict[str, Any]) -> Optional[ReagentDB]:
        """Update reagent properties"""
        reagent = db.query(ReagentDB).filter(ReagentDB.id == reagent_id).first()
        if reagent:
            # Don't allow ID updates
            update_data = update_data.copy()
            update_data.pop('id', None)
            
            for key, value in update_data.items():
                if key in ["aliases", "properties", "components", "documentation"]:
                    setattr(reagent, key, json.dumps(value))
                elif hasattr(reagent, key):
                    setattr(reagent, key, value)
            db.commit()
            db.refresh(reagent)
        return reagent
    
    @staticmethod
    def delete_reagent(db: Session, reagent_id: int) -> bool:
        """Delete reagent and all associated tubes"""
        reagent = db.query(ReagentDB).filter(ReagentDB.id == reagent_id).first()
        if reagent:
            db.delete(reagent)
            db.commit()
            return True
        return False
    
    @staticmethod
    def create_tube(db: Session, tube_data: Dict[str, Any]) -> TubeDB:
        """Create a new tube - ID is auto-generated"""
        # Remove id if provided by user - let database handle it
        tube_data = tube_data.copy()
        tube_data.pop('id', None)  # Remove id if it exists
        
        tube = TubeDB(
            reagent_id=tube_data.get("reagent_id"),
            capacity=tube_data["capacity"],
            volume=tube_data.get("volume", 0),
            label=tube_data["label"],
            status=tube_data.get("status", "available"),
            location_x=tube_data.get("location_x", 0),
            location_y=tube_data.get("location_y", 0),
            location_area=tube_data.get("location_area", Area.UNKNOWN),
            is_casework=tube_data.get("is_casework", True)
        )
        db.add(tube)
        db.commit()
        db.refresh(tube)  # This will load the auto-generated ID
        return tube
    
    @staticmethod
    def get_tube(db: Session, tube_id: int) -> Optional[TubeDB]:
        """Get tube by ID"""
        return db.query(TubeDB).filter(TubeDB.id == tube_id).first()
    
    @staticmethod
    def get_tubes_for_reagent(db: Session, reagent_id: int, is_casework: Optional[bool] = None) -> List[TubeDB]:
        """Get tubes for a specific reagent"""
        query = db.query(TubeDB).filter(TubeDB.reagent_id == reagent_id)
        if is_casework is not None:
            query = query.filter(TubeDB.is_casework == int(is_casework))
        return query.all()
    
    @staticmethod
    def update_tube_volume(db: Session, tube_id: int, new_volume: float) -> Optional[TubeDB]:
        """Update tube volume"""
        tube = db.query(TubeDB).filter(TubeDB.id == tube_id).first()
        if tube and new_volume <= tube.capacity:
            tube.volume = new_volume
            tube.status = "depleted" if new_volume == 0 else "available"
            db.commit()
            db.refresh(tube)
            return tube
        return None
    
    @staticmethod
    def move_tube(db: Session, tube_id: int, to_casework: bool, area: Area = Area.UNKNOWN) -> Optional[TubeDB]:
        """Move tube between casework and repository"""
        tube = db.query(TubeDB).filter(TubeDB.id == tube_id).first()
        if tube:
            tube.is_casework = int(to_casework)
            tube.location_area = area
            db.commit()
            db.refresh(tube)
            return tube
        return None
    
    @staticmethod
    def get_tubes_by_area(db: Session, area: Area) -> List[TubeDB]:
        """Get all tubes in a specific area"""
        return db.query(TubeDB).filter(TubeDB.location_area == area).all()
    
    @staticmethod
    def bulk_import_reagents(db: Session, reagents_json: List[Dict[str, Any]]) -> List[ReagentDB]:
        """Import multiple reagents from JSON data (like the original schema examples)"""
        created_reagents = []
        for reagent_data in reagents_json:
            try:
                # Use the schema-aware creation method
                reagent = DatabaseManager.create_reagent_from_json_schema(db, reagent_data)
                created_reagents.append(reagent)
                print(f"Created reagent '{reagent.name}' with auto-generated ID: {reagent.id}")
            except ValueError as e:
                print(f"Skipping reagent: {e}")
                continue
        return created_reagents