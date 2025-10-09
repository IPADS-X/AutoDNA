#pragma once

#include <cstdint>

static const uint16_t SN_LEN     = 5;
static const uint16_t TUBE_LEN   = 96;
static const uint16_t RESULT_LEN = 2 * TUBE_LEN;

class AmplificationModbus {
public:
    static const int START_ADDR = 40233;

    static const int READY_ADDR    = 40000;
    static const int READY_SN_ADDR = READY_ADDR + 1;

    static const int NOTIFY_ADDR           = READY_SN_ADDR + SN_LEN;
    static const int NOTIFY_SN_ADDR        = NOTIFY_ADDR + 1;
    static const int NOTIFY_TUBE_ADDR      = NOTIFY_SN_ADDR + SN_LEN;
    static const int NOTIFY_VOLUME_ADDR    = 40234;
    static const int NOTIFY_PCR_FILE_ADDR  = 40235;
    static const int NOTIFY_PIPETTING_ADDR = 40236;
    static const int NOTIFY_VOLUME2_ADDR   = 40237;

    static const int FINISH_ADDR        = NOTIFY_TUBE_ADDR + TUBE_LEN;
    static const int FINISH_STATUS_ADDR = FINISH_ADDR + 1;
    static const int FINISH_OLD_SN_ADDR = FINISH_STATUS_ADDR + 1;
    static const int FINISH_NEW_SN_ADDR = FINISH_OLD_SN_ADDR + SN_LEN;
    static const int FINISH_TUBE_ADDR   = FINISH_NEW_SN_ADDR + SN_LEN;

    static const int FINISH_CONFIRM_ADDR    = FINISH_TUBE_ADDR + TUBE_LEN;
    static const int FINISH_CONFIRM_SN_ADDR = FINISH_CONFIRM_ADDR + 1;
};

class RefrigeratorModbus {
public:
    static const int READY_ADDR    = 40000;
    static const int READY_SN_ADDR = READY_ADDR + 1;

    static const int START_ADDR      = READY_SN_ADDR + SN_LEN;
    static const int START_SN_ADDR   = START_ADDR + 1;
    static const int START_TUBE_ADDR = START_SN_ADDR + SN_LEN;

    static const int FINISH_ADDR        = START_TUBE_ADDR + TUBE_LEN;
    static const int FINISH_STATUS_ADDR = FINISH_ADDR + 1;
    static const int FINISH_OLD_SN_ADDR = FINISH_STATUS_ADDR + 1;
    static const int FINISH_NEW_SN_ADDR = FINISH_OLD_SN_ADDR + SN_LEN;
    static const int FINISH_TUBE_ADDR   = FINISH_NEW_SN_ADDR + SN_LEN;

    static const int FINISH_CONFIRM_ADDR    = FINISH_TUBE_ADDR + TUBE_LEN;
    static const int FINISH_CONFIRM_SN_ADDR = FINISH_CONFIRM_ADDR + 1;
};

class CentrifugalModbus {
public:
    static const int START_ADDR = 40426;

    static const int READY_ADDR    = 40000;
    static const int READY_SN_ADDR = READY_ADDR + 1;

    static const int NOTIFY_ADDR          = READY_SN_ADDR + SN_LEN;
    static const int NOTIFY_SN_ADDR       = NOTIFY_ADDR + 1;
    static const int NOTIFY_TUBE_ADDR     = NOTIFY_SN_ADDR + SN_LEN;
    static const int NOTIFY_MODE_ADDR     = NOTIFY_TUBE_ADDR + TUBE_LEN;
    static const int NOTIFY_SPEED_ADDR    = 40427;
    static const int NOTIFY_DURATION_ADDR = 40428;

    static const int FINISH_ADDR        = 40114;
    static const int FINISH_STATUS_ADDR = FINISH_ADDR + 1;
    static const int FINISH_SN_ADDR     = FINISH_STATUS_ADDR + 1;
    static const int FINISH_TUBE_ADDR   = FINISH_SN_ADDR + SN_LEN;

    static const int FINISH_CONFIRM_ADDR    = 40410;
    static const int FINISH_CONFIRM_SN_ADDR = 40412;
};

class PortageModbus {
public:
    static const int START_ADDR     = 40019;
    static const int HEARTBEAT_ADDR = 40000;
    static const int READY_ADDR     = 40001;

    static const int NOTIFY_ADDR             = READY_ADDR + 1;
    static const int NOTIFY_MODE_ADDR        = NOTIFY_ADDR + 1;
    static const int NOTIFY_SRC_ADDR         = NOTIFY_MODE_ADDR + 1;
    static const int NOTIFY_DEST_ADDR        = NOTIFY_SRC_ADDR + 1;
    static const int NOTIFY_MANUAL_POS_ADDR  = NOTIFY_DEST_ADDR + 1;
    static const int NOTIFY_MANUAL_DEST_ADDR = NOTIFY_MANUAL_POS_ADDR + 1;
    static const int NOTIFY_MANUAL_SN_ADDR   = NOTIFY_MANUAL_DEST_ADDR + 1;

    static const int REAL_FINISH_ADDR = 40018;

    static const int FINISH_ADDR = 40020;
};

class FluorescenceModbus {
public:
    static const int START_ADDR = 40343;

    static const int READY_ADDR    = 40000;
    static const int READY_SN_ADDR = 40103;

    static const int NOTIFY_ADDR       = 40001;
    static const int NOTIFY_SN_ADDR    = 40113;
    static const int NOTIFY_TUBE_ADDR  = 40007;
    static const int NOTIFY_COVER_ADDR = 40342;
    static const int NOTIFY_TIME_ADDR  = 40344;

    static const int FINISH_ADDR        = 40002;
    static const int FINISH_STATUS_ADDR = 40003;
    static const int FINISH_SN_ADDR     = 40123;
    static const int FINISH_RESULT_ADDR = 40146;

    static const int FINISH_CONFIRM_ADDR    = 40004;
    static const int FINISH_CONFIRM_SN_ADDR = 40133;
};

class ConnecctPortageModbus {
public:
    static const int START_ADDR  = 50026;
    static const int FINISH_ADDR = 50076;

    static const int MODE_ADDR = 52600;
};

class LibMoveCarrierModbus {
public:
    static const int START_ADDR  = 50018;
    static const int FINISH_ADDR = 50068;

    static const int START_POS_ADDR = 51800;
    static const int END_POS_ADDR   = 51801;
};

class LibMoveTubeModbus {
public:
    static const int START_ADDR  = 50013;
    static const int FINISH_ADDR = 50063;

    static const int START_POS_ADDR   = 51300;
    static const int START_INDEX_ADDR = 51301;
    static const int END_POS_ADDR     = 51302;
    static const int END_INDEX_ADDR   = 51303;
};

class LibMovePcrTubeModbus {
public:
    static const int START_ADDR  = 50029;
    static const int FINISH_ADDR = 50079;

    static const int START_POS_ADDR   = 52900;
    static const int START_INDEX_ADDR = 52901;
    static const int END_POS_ADDR     = 52902;
    static const int END_INDEX_ADDR   = 52903;
};

class LibSetHeaterTempModbus {
public:
    static const int START_ADDR  = 50019;
    static const int FINISH_ADDR = 50069;

    static const int TEMP_ADDR = 51900;
};

class LibAspirateMixModbus {
public:
    static const int START_ADDR  = 50020;
    static const int FINISH_ADDR = 50070;

    static const int POS_ADDR       = 52000;
    static const int INDEX_ADDR     = 52001;
    static const int VOLUME_ADDR    = 52002;
    static const int TOTAL_ADDR     = 52003;
    static const int NUM_ADDR       = 52004;
    static const int TR_ADDR        = 52005;
    static const int MIX_SPEED_ADDR = 52006;
};

class LibPipetteModbus {
public:
    static const int START_ADDR  = 50012;
    static const int FINISH_ADDR = 50062;

    static const int START_POS_ADDR        = 51200;
    static const int START_INDEX_ADDR      = 51201;
    static const int VOLUME_ADDR           = 51202;
    static const int END_POS_ADDR          = 51203;
    static const int END_INDEX_ADDR        = 51204;
    static const int NUM_ADDR              = 51205;
    static const int TR_ADDR               = 51206;
    static const int MIX_TIME_ADDR         = 51207;
    static const int MIX_VOLUME_ADDR       = 51208;
    static const int AFTER_MIX_ADDR        = 51209;
    static const int AFTER_MIX_VOLUME_ADDR = 51210;
    static const int MIX_SPEED_ADDR        = 51211;
};

class LibCentrifugeModbus {
public:
    static const int START_ADDR  = 50025;
    static const int FINISH_ADDR = 50075;

    static const int DURATION_ADDR = 52500;
    static const int SPEED_ADDR    = 52501;
    static const int TYPE_ADDR     = 52502; // 0: 8-tube, 1: PCR tube
};

class LibTimeModbus {
public:
    static const int START_ADDR  = 50022;
    static const int FINISH_ADDR = 50072;

    static const int DURATION_ADDR = 52200;
    static const int TYPE_ADDR     = 52201;

    enum class TimeType { MILLISECOND, SECOND, MINUTE };
};

class FluoMoveCarrierModbus {
public:
    static const int START_ADDR  = 50018;
    static const int FINISH_ADDR = 50068;

    static const int START_POS_ADDR = 51800;
    static const int END_POS_ADDR   = 51801;
};

class FluoMoveTubeModbus {
public:
    static const int START_ADDR  = 50013;
    static const int FINISH_ADDR = 50063;

    static const int START_POS_ADDR   = 51300;
    static const int START_INDEX_ADDR = 51301;
    static const int END_POS_ADDR     = 51302;
    static const int END_INDEX_ADDR   = 51303;
};

class FluoPipetteModbus {
public:
    static const int START_ADDR  = 50012;
    static const int FINISH_ADDR = 50062;

    static const int START_POS_ADDR        = 51200;
    static const int START_INDEX_ADDR      = 51201;
    static const int VOLUME_ADDR           = 51202;
    static const int END_POS_ADDR          = 51203;
    static const int END_INDEX_ADDR        = 51204;
    static const int NUM_ADDR              = 51205;
    static const int TR_ADDR               = 51206;
    static const int MIX_TIME_ADDR         = 51207;
    static const int MIX_VOLUME_ADDR       = 51208;
    static const int AFTER_MIX_ADDR        = 51209;
    static const int AFTER_MIX_VOLUME_ADDR = 51210;
    static const int MIX_SPEED_ADDR        = 51211;
};

class FluoCapTubeModbus {
public:
    static const int START_ADDR  = 50025;
    static const int FINISH_ADDR = 50075;
};

class FluoFluoModbus {
public:
    static const int START_ADDR  = 50029;
    static const int FINISH_ADDR = 50079;
};

class AmpMoveCarrierModbus {
public:
    static const int START_ADDR  = 50018;
    static const int FINISH_ADDR = 50068;

    static const int START_POS_ADDR = 51800;
    static const int END_POS_ADDR   = 51801;
};

class AmpMoveTubeModbus {
public:
    static const int START_ADDR  = 50013;
    static const int FINISH_ADDR = 50063;

    static const int START_POS_ADDR   = 51300;
    static const int START_INDEX_ADDR = 51301;
    static const int END_POS_ADDR     = 51302;
    static const int END_INDEX_ADDR   = 51303;
};

class AmpPcrModbus {
public:
    static const int START_ADDR  = 50021;
    static const int FINISH_ADDR = 50071;

    static const int FILE_ADDR = 52100;
};

class AmpAspirateMixModbus {
public:
    static const int START_ADDR  = 50020;
    static const int FINISH_ADDR = 50070;

    static const int POS_ADDR       = 52000;
    static const int INDEX_ADDR     = 52001;
    static const int VOLUME_ADDR    = 52002;
    static const int TOTAL_ADDR     = 52003;
    static const int NUM_ADDR       = 52004;
    static const int TR_ADDR        = 52005;
    static const int MIX_SPEED_ADDR = 52006;
};

class PuriPipetteModbus {
public:
    static const int START_ADDR  = 50012;
    static const int FINISH_ADDR = 50062;

    static const int START_POS_ADDR        = 51200;
    static const int START_INDEX_ADDR      = 51201;
    static const int VOLUME_ADDR           = 51202;
    static const int END_POS_ADDR          = 51203;
    static const int END_INDEX_ADDR        = 51204;
    static const int NUM_ADDR              = 51205;
    static const int TR_ADDR               = 51206;
    static const int MIX_TIME_ADDR         = 51207;
    static const int MIX_VOLUME_ADDR       = 51208;
    static const int AFTER_MIX_ADDR        = 51209;
    static const int AFTER_MIX_VOLUME_ADDR = 51210;
    static const int MIX_SPEED_ADDR        = 51211;
};

class PuriMoveTubeModbus {
public:
    static const int START_ADDR  = 50013;
    static const int FINISH_ADDR = 50063;

    static const int START_POS_ADDR   = 51300;
    static const int START_INDEX_ADDR = 51301;
    static const int END_POS_ADDR     = 51302;
    static const int END_INDEX_ADDR   = 51303;
};

class PuriMoveCarrierModbus {
public:
    static const int START_ADDR  = 50018;
    static const int FINISH_ADDR = 50068;

    static const int START_POS_ADDR = 51800;
    static const int END_POS_ADDR   = 51801;
};

class PuriAspirateMixModbus {
public:
    static const int START_ADDR  = 50020;
    static const int FINISH_ADDR = 50070;

    static const int POS_ADDR       = 52000;
    static const int INDEX_ADDR     = 52001;
    static const int VOLUME_ADDR    = 52002;
    static const int TOTAL_ADDR     = 52003;
    static const int NUM_ADDR       = 52004;
    static const int TR_ADDR        = 52005;
    static const int MIX_SPEED_ADDR = 52006;
};

class PuriShakeModbus {
public:
    static const int START_ADDR  = 50019;
    static const int FINISH_ADDR = 50069;

    static const int DURATION_ADDR = 51900;
    static const int SPEED_ADDR    = 51901;
    static const int TEMP_ADDR     = 51902;
};

class PuriCentrifugeModbus {
public:
    static const int START_ADDR  = 50025;
    static const int FINISH_ADDR = 50075;

    static const int DURATION_ADDR = 52500;
    static const int SPEED_ADDR    = 52501;
};

class PuriTimeModbus {
public:
    static const int START_ADDR  = 50022;
    static const int FINISH_ADDR = 50072;

    static const int DURATION_ADDR = 52200;
    static const int TYPE_ADDR     = 52201;

    enum class TimeType { MILLISECOND, SECOND, MINUTE };
};

class PuriPcrModbus {
public:
    static const int START_ADDR  = 50021;
    static const int FINISH_ADDR = 50071;

    static const int FILE_ADDR = 52100;
};
