# ==================== api.py ====================
"""FastAPI REST API server"""

from fastapi import FastAPI, HTTPException, Depends, Query
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any
from sqlalchemy.orm import Session
import uvicorn
from database import (
    DatabaseManager, SessionLocal, Area, TubeType,
    ReagentDB, TubeDB
)

# Create FastAPI app
app = FastAPI(
    title="Reagent Manager API",
    description="API for managing laboratory reagents and tubes",
    version="1.0.0"
)

# Add CORS middleware for cross-origin requests
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # In production, specify actual origins
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ==================== Pydantic Models for API ====================

class ReagentCreate(BaseModel):
    """Model for creating a reagent"""
    name: str
    category: str
    form: str
    aliases: List[str] = []
    properties: Dict[str, Any] = {}
    components: List[Dict[str, Any]] = []
    documentation: List[str] = []
    notes: Optional[str] = None

class ReagentUpdate(BaseModel):
    """Model for updating a reagent"""
    name: Optional[str] = None
    category: Optional[str] = None
    form: Optional[str] = None
    aliases: Optional[List[str]] = None
    properties: Optional[Dict[str, Any]] = None
    components: Optional[List[Dict[str, Any]]] = None
    documentation: Optional[List[str]] = None
    notes: Optional[str] = None

class TubeCreate(BaseModel):
    """Model for creating a tube"""
    reagent_id: Optional[int] = None
    capacity: int
    volume: float = 0
    label: str
    status: str = "available"
    location_x: int = 0
    location_y: int = 0
    location_area: int = 0
    is_casework: bool = True

class TubeAllocation(BaseModel):
    """Model for tube allocation"""
    count: int = 1
    capacity: int = 200
    volume: float = 0
    is_casework: bool = True
    area: int = 0

class ReagentWithTubes(BaseModel):
    """Model for creating reagent with tube allocations"""
    reagent: ReagentCreate
    tube_allocations: List[TubeAllocation] = []

class TubeVolumeUpdate(BaseModel):
    """Model for updating tube volume"""
    volume: float

class TubeMove(BaseModel):
    """Model for moving a tube"""
    to_casework: bool
    area: int = 0

# ==================== API Endpoints ====================

@app.get("/")
async def root():
    """Root endpoint"""
    return {
        "service": "Reagent Manager API",
        "status": "running",
        "endpoints": {
            "reagents": "/reagents",
            "tubes": "/tubes",
            "docs": "/docs"
        }
    }

# --- Reagent Endpoints ---

@app.post("/reagents", response_model=Dict[str, Any])
async def create_reagent(
    request: ReagentWithTubes,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Create a new reagent with optional tube allocations"""
    try:
        # Create reagent
        reagent_dict = request.reagent.model_dump()
        reagent_db = DatabaseManager.create_reagent(db, reagent_dict)
        
        # Allocate tubes if specified
        if request.tube_allocations:
            for allocation in request.tube_allocations:
                for _ in range(allocation.count):
                    tube_data = {
                        "reagent_id": reagent_db.id,
                        "capacity": allocation.capacity,
                        "volume": allocation.volume,
                        "label": reagent_db.name,
                        "is_casework": allocation.is_casework,
                        "location_area": allocation.area
                    }
                    DatabaseManager.create_tube(db, tube_data)
        
        # Get the created reagent with tubes
        result = reagent_db.to_dict()
        
        # Add tube information
        casework_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_db.id, is_casework=True)
        repo_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_db.id, is_casework=False)
        
        result["tubes"] = {
            "casework": [t.to_dict() for t in casework_tubes],
            "repository": [t.to_dict() for t in repo_tubes],
            "total_count": len(casework_tubes) + len(repo_tubes)
        }
        
        return result
        
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/reagents", response_model=List[Dict[str, Any]])
async def get_all_reagents(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=1000),
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get all reagents with pagination"""
    reagents = DatabaseManager.get_all_reagents(db, skip, limit)
    return [r.to_dict() for r in reagents]

@app.get("/reagents/{reagent_id}", response_model=Dict[str, Any])
async def get_reagent(
    reagent_id: int,
    include_tubes: bool = Query(False),
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get reagent by ID"""
    reagent = DatabaseManager.get_reagent(db, reagent_id)
    if not reagent:
        raise HTTPException(status_code=404, detail="Reagent not found")
    
    result = reagent.to_dict()
    
    if include_tubes:
        casework_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_id, is_casework=True)
        repo_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_id, is_casework=False)
        
        result["tubes"] = {
            "casework": [t.to_dict() for t in casework_tubes],
            "repository": [t.to_dict() for t in repo_tubes],
            "casework_volume": sum(t.volume for t in casework_tubes),
            "repository_volume": sum(t.volume for t in repo_tubes)
        }
    
    return result

@app.get("/reagents/name/{name}", response_model=Dict[str, Any])
async def get_reagent_by_name(
    name: str,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get reagent by name"""
    reagent = DatabaseManager.get_reagent_by_name(db, name)
    if not reagent:
        raise HTTPException(status_code=404, detail="Reagent not found")
    return reagent.to_dict()

@app.patch("/reagents/{reagent_id}", response_model=Dict[str, Any])
async def update_reagent(
    reagent_id: int,
    update: ReagentUpdate,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Update reagent properties"""
    update_data = {k: v for k, v in update.dict().items() if v is not None}
    reagent = DatabaseManager.update_reagent(db, reagent_id, update_data)
    if not reagent:
        raise HTTPException(status_code=404, detail="Reagent not found")
    return reagent.to_dict()

@app.delete("/reagents/{reagent_id}")
async def delete_reagent(
    reagent_id: int,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Delete reagent and all associated tubes"""
    if not DatabaseManager.delete_reagent(db, reagent_id):
        raise HTTPException(status_code=404, detail="Reagent not found")
    return {"message": "Reagent deleted successfully"}

# --- Tube Endpoints ---

@app.post("/tubes", response_model=Dict[str, Any])
async def create_tube(
    tube: TubeCreate,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Create a new tube"""
    try:
        tube_db = DatabaseManager.create_tube(db, tube.dict())
        return tube_db.to_dict()
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/tubes/reagent/{reagent_id}", response_model=List[Dict[str, Any]])
async def get_tubes_for_reagent(
    reagent_id: int,
    location: Optional[str] = Query(None, regex="^(casework|repository)$"),
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get tubes for a specific reagent"""
    is_casework = None
    if location == "casework":
        is_casework = True
    elif location == "repository":
        is_casework = False
    
    tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_id, is_casework)
    return [t.to_dict() for t in tubes]

@app.patch("/tubes/{tube_id}/volume", response_model=Dict[str, Any])
async def update_tube_volume(
    tube_id: int,
    update: TubeVolumeUpdate,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Update tube volume"""
    tube = DatabaseManager.update_tube_volume(db, tube_id, update.volume)
    if not tube:
        raise HTTPException(status_code=404, detail="Tube not found or volume exceeds capacity")
    return tube.to_dict()

@app.patch("/tubes/{tube_id}/move", response_model=Dict[str, Any])
async def move_tube(
    tube_id: int,
    move: TubeMove,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Move tube between casework and repository"""
    tube = DatabaseManager.move_tube(db, tube_id, move.to_casework, Area(move.area))
    if not tube:
        raise HTTPException(status_code=404, detail="Tube not found")
    return tube.to_dict()

@app.get("/tubes/area/{area}", response_model=List[Dict[str, Any]])
async def get_tubes_by_area(
    area: int,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get all tubes in a specific area"""
    tubes = DatabaseManager.get_tubes_by_area(db, Area(area))
    return [t.to_dict() for t in tubes]

# --- Summary Endpoints ---

@app.get("/summary/reagent/{reagent_id}", response_model=Dict[str, Any])
async def get_reagent_summary(
    reagent_id: int,
    db: Session = Depends(DatabaseManager.get_db)
):
    """Get complete summary for a reagent including all tubes"""
    reagent = DatabaseManager.get_reagent(db, reagent_id)
    if not reagent:
        raise HTTPException(status_code=404, detail="Reagent not found")
    
    casework_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_id, is_casework=True)
    repo_tubes = DatabaseManager.get_tubes_for_reagent(db, reagent_id, is_casework=False)
    
    # Group tubes by area
    tubes_by_area = {}
    all_tubes = casework_tubes + repo_tubes
    for tube in all_tubes:
        area_name = Area(tube.location_area).name
        if area_name not in tubes_by_area:
            tubes_by_area[area_name] = []
        tubes_by_area[area_name].append(tube.to_dict())
    
    summary = reagent.to_dict()
    summary["tube_summary"] = {
        "total_count": len(all_tubes),
        "casework_count": len(casework_tubes),
        "repository_count": len(repo_tubes),
        "total_volume": sum(t.volume for t in all_tubes),
        "casework_volume": sum(t.volume for t in casework_tubes),
        "repository_volume": sum(t.volume for t in repo_tubes),
        "by_area": tubes_by_area
    }
    
    return summary

# ==================== Server Runner ====================

def run_server(host: str = "127.0.0.1", port: int = 8000):
    """Run the FastAPI server"""
    uvicorn.run(app, host=host, port=port, reload=False)

if __name__ == "__main__":
    print("Starting Reagent Manager API Server...")
    print("Access the API at: http://127.0.0.1:8000")
    print("Interactive docs at: http://127.0.0.1:8000/docs")
    run_server()

