using System;
using System.Text;
using System.Runtime.InteropServices;
#pragma warning disable 1591
#pragma warning disable 0169




namespace PCI_DMC
{
    public class CPCI_DMC
    {
        public struct IO_Information   //MotionBuffer
        {
           ushort NodeID;
	       ushort SlotID;
	       ushort BitNo;
        }
        public struct IO_Set   //MotionBuffer
        {
            IO_Information IO_Info;
            ushort Value;
        }


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_cardtype_errorcode")]
        public static extern short CS_DMC_01_get_cardtype_errorcode(ref ushort returncode);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_clear_cardtype_errorcode")]
        public static extern short CS_DMC_01_clear_cardtype_errorcode();

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_open")]
        public static extern short CS_DMC_01_open(ref short existcard);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_CardNo_seq")]
        public static extern short CS_DMC_01_get_CardNo_seq(ushort CardNo_seq, ref ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_close")]
        public static extern void CS_DMC_01_close();
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_pci_initial")]
        public static extern short CS_DMC_01_pci_initial(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_initial_bus")]
        public static extern short CS_DMC_01_initial_bus(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_initial_bus2")]
        public static extern short CS_DMC_01_initial_bus2();
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ring")]
        public static extern short CS_DMC_01_start_ring(ushort CardNo, byte RingNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_card_version")]
        public static extern short CS_DMC_01_get_card_version(ushort CardNo, ref ushort ver);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_devicetype")]
        public static extern short CS_DMC_01_get_devicetype(short CardNo, ushort NodeID, ushort SlotID, ref uint DeviceType, ref uint IdentityObject);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_device_table")]
        public static extern short CS_DMC_01_get_device_table(ushort CardNo, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_node_table")]
        public static extern short CS_DMC_01_get_node_table(ushort CardNo, ref uint NodeIDTable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_cycle_time")]
        public static extern short CS_DMC_01_get_cycle_time(ushort CardNo, ref int time);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_check_card_running")]
        public static extern short CS_DMC_01_check_card_running(ushort CardNo, ref ushort running);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_reset_card")]
        public static extern short CS_DMC_01_reset_card(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_check_nodeno")]
        public static extern short CS_DMC_01_check_nodeno(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort exist);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_master_connect_status")]
        public static extern short CS_DMC_01_get_master_connect_status(ushort CardNo, ref ushort Protocal);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_mailbox_Error")]
        public static extern short CS_DMC_01_get_mailbox_Error(ushort CardNo, ref uint error_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_mailbox_cnt")]
        public static extern short CS_DMC_01_get_mailbox_cnt(ushort CardNo, ref uint PC_cnt, ref uint DSP_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dsp_cnt")]
        public static extern short CS_DMC_01_get_dsp_cnt(ushort CardNo, ref uint int_cnt, ref uint main_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_output")]
        public static extern short CS_DMC_01_set_dio_output(ushort CardNo, ushort On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_output")]
        public static extern short CS_DMC_01_get_dio_output(ushort CardNo, ref ushort On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_input")]
        public static extern short CS_DMC_01_get_dio_input(ushort CardNo, ref ushort On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_check_canopen_lock")]
        public static extern short CS_DMC_01_check_canopen_lock(ushort CardNo, ref ushort lock_);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_canopen_ret")]
        public static extern short CS_DMC_01_get_canopen_ret(ushort CardNo, ref ushort COBID, ref byte value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_send_message")]
        public static extern short CS_DMC_01_send_message(ushort CardNo, ushort NodeID, ushort SlotID, ushort Index, ushort SubIdx, ushort DataType, ushort Value0, ushort Value1, ushort Value2, ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_send_message2")]
        public static extern short CS_DMC_01_send_message2(ushort CardNo, ushort NodeID, ushort SlotID, ushort Index, ushort SubIdx, ushort DataType, ushort Value0, ushort Value1, ushort Value2, ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_send_message3")]
        public static extern short CS_DMC_01_send_message3(short CardNo, ref ushort Index, ref ushort SubIdx, ref ushort DataType, ref ushort Value0, ref ushort Value1, ref ushort Value2, ref ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_send_message4")]
        public static extern short CS_DMC_01_send_message4(ushort CardNo, ushort NodeID, ushort SlotID, ushort Index, ushort SubIdx, ushort DataType, ushort Value0, ushort Value1, ushort Value2, ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_reset_sdo_choke")]
        public static extern short CS_DMC_01_reset_sdo_choke(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_sdo_retry_history")]
        public static extern short CS_DMC_01_get_sdo_retry_history(ushort CardNo, ref uint cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_message")]
        public static extern short CS_DMC_01_read_message(short CardNo, ref ushort Cmd, ref ushort COBID, ref ushort SubIdx, ref ushort Value0, ref ushort Value1, ref ushort Value2, ref ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_message2")]
        public static extern short CS_DMC_01_read_message2(short CardNo, ushort NodeID, ref ushort Cmd, ref ushort COBID, ref ushort SubIdx, ref ushort Value0, ref ushort Value1, ref ushort Value2, ref ushort Value3, ref ushort cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_message")]
        public static extern short CS_DMC_01_get_message(short CardNo, ushort NodeID, ushort SlotID, ushort Index, ushort SubIdx, ref ushort Cmd, ref ushort COBID, ref ushort SubIndex, ref ushort Value0, ref ushort Value1, ref ushort Value2, ref ushort Value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_sdo_driver_speed_profile")]
        public static extern short CS_DMC_01_set_sdo_driver_speed_profile(ushort CardNo, ushort NodeID, ushort SlotID, uint MaxVel, double acc, double dec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sdo_driver_r_move")]
        public static extern short CS_DMC_01_start_sdo_driver_r_move(ushort CardNo, ushort NodeID, ushort SlotID, int Distance);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sdo_driver_a_move")]
        public static extern short CS_DMC_01_start_sdo_driver_a_move(short CardNo, ushort NodeID, ushort SlotID, int Position);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sdo_driver_new_position_move")]
        public static extern short CS_DMC_01_start_sdo_driver_new_position_move(short CardNo, ushort NodeID, ushort SlotID, int Position, ushort abs_rel);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_config")]
        public static extern short CS_DMC_01_set_home_config(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode, int offset, ushort lowSpeed, ushort highSpeed, double acc);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_move")]
        public static extern short CS_DMC_01_set_home_move(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_config2")]
        public static extern short CS_DMC_01_set_home_config2(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode, int offset, ushort lowSpeed, ushort highSpeed, double acc);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_config_offset2")]
        public static extern short CS_DMC_01_set_home_config_offset2(ushort CardNo, ushort NodeID, ushort SlotID, int offset);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_move2")]
        public static extern short CS_DMC_01_set_home_move2(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_escape_home_move")]
        public static extern short CS_DMC_01_escape_home_move(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_ipo_set_svon")]
        public static extern short CS_DMC_01_ipo_set_svon(ushort CardNo, ushort NodeID, ushort SlotID, ushort ON_OFF);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_motion_cnt")]
        public static extern short CS_DMC_01_motion_cnt(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort pc_mc_cnt, ref ushort dsp_mc_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_move")]
        public static extern short CS_DMC_01_start_tr_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_move")]
        public static extern short CS_DMC_01_start_sr_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_move")]
        public static extern short CS_DMC_01_start_ta_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_move")]
        public static extern short CS_DMC_01_start_sa_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_p_change")]
        public static extern short CS_DMC_01_p_change(ushort CardNo, ushort NodeID, ushort SlotID, int NewPos);
        //20160725
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_p_change_multi_axes")]
        public static extern short CS_DMC_01_p_change_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int position, int MaxVel, int EndVel, double Tacc, double Tdec, short Curve_mode);


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_v_change")]
        public static extern short CS_DMC_01_v_change(ushort CardNo, ushort NodeID, ushort SlotID, int NewSpeed, double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_move_2seg")]
        public static extern short CS_DMC_01_start_tr_move_2seg(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_move_2seg")]
        public static extern short CS_DMC_01_start_sr_move_2seg(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_move_2seg")]
        public static extern short CS_DMC_01_start_ta_move_2seg(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_move_2seg")]
        public static extern short CS_DMC_01_start_sa_move_2seg(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_move_2seg2")]
        public static extern short CS_DMC_01_start_tr_move_2seg2(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_move_2seg2")]
        public static extern short CS_DMC_01_start_sr_move_2seg2(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_move_2seg2")]
        public static extern short CS_DMC_01_start_ta_move_2seg2(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_move_2seg2")]
        public static extern short CS_DMC_01_start_sa_move_2seg2(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int Dist2, int StrVel, int MaxVel, int MaxVel2, double Tacc, double Tsec, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_move_xy")]
        public static extern short CS_DMC_01_start_tr_move_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_move_xy")]
        public static extern short CS_DMC_01_start_sr_move_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_move_xy")]
        public static extern short CS_DMC_01_start_ta_move_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_move_xy")]
        public static extern short CS_DMC_01_start_sa_move_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_arc_xy")]
        public static extern short CS_DMC_01_start_tr_arc_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_arc_xy")]
        public static extern short CS_DMC_01_start_sr_arc_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_arc_xy")]
        public static extern short CS_DMC_01_start_ta_arc_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_arc_xy")]
        public static extern short CS_DMC_01_start_sa_arc_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_arc2_xy")]
        public static extern short CS_DMC_01_start_tr_arc2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int End_X, int End_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_arc2_xy")]
        public static extern short CS_DMC_01_start_sr_arc2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int End_X, int End_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_arc2_xy")]
        public static extern short CS_DMC_01_start_ta_arc2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int End_X, int End_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_arc2_xy")]
        public static extern short CS_DMC_01_start_sa_arc2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int End_X, int End_Y, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_arc3_xy")]
        public static extern short CS_DMC_01_start_tr_arc3_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int End_X, int End_Y, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_arc3_xy")]
        public static extern short CS_DMC_01_start_sr_arc3_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int End_X, int End_Y, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_arc3_xy")]
        public static extern short CS_DMC_01_start_ta_arc3_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int End_X, int End_Y, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_arc3_xy")]
        public static extern short CS_DMC_01_start_sa_arc3_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int End_X, int End_Y, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_move_xyz")]
        public static extern short CS_DMC_01_start_tr_move_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int DisZ, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_move_xyz")]
        public static extern short CS_DMC_01_start_sr_move_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int DisZ, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_move_xyz")]
        public static extern short CS_DMC_01_start_ta_move_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int DisZ, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_move_xyz")]
        public static extern short CS_DMC_01_start_sa_move_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int DisZ, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tr_heli_xy")]
        public static extern short CS_DMC_01_start_tr_heli_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int Depth, int Pitch, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sr_heli_xy")]
        public static extern short CS_DMC_01_start_sr_heli_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int Depth, int Pitch, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_ta_heli_xy")]
        public static extern short CS_DMC_01_start_ta_heli_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int Depth, int Pitch, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_sa_heli_xy")]
        public static extern short CS_DMC_01_start_sa_heli_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int Depth, int Pitch, short Dir, int StrVel, int MaxVel, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_tv_move")]
        public static extern short CS_DMC_01_tv_move(ushort CardNo, ushort NodeID, ushort SlotID, int StrVel, int MaxVel, double Tacc, short Dir);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sv_move")]
        public static extern short CS_DMC_01_sv_move(ushort CardNo, ushort NodeID, ushort SlotID, int StrVel, int MaxVel, double Tacc, short Dir);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_motion_error_code")]
        public static extern short CS_DMC_01_get_motion_error_code(ushort CardNo, ushort NodeID, ushort SlotID, ref short error_code);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_motion_error_cnt")]
        public static extern short CS_DMC_01_get_motion_error_cnt(ushort CardNo, ushort NodeID, ushort SlotID, ref short error_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_emg_stop")]
        public static extern short CS_DMC_01_emg_stop(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sd_stop")]
        public static extern short CS_DMC_01_sd_stop(ushort CardNo, ushort NodeID, ushort SlotID, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sd_abort")]
        public static extern short CS_DMC_01_sd_abort(ushort CardNo, ushort NodeID, ushort SlotID, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_motion_done")]
        public static extern short CS_DMC_01_motion_done(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort MC_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_motion_done_multi_axes")]
        public static extern short CS_DMC_01_motion_done_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref ushort MC_status);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_motion_status")]
        public static extern short CS_DMC_01_motion_status(ushort CardNo, ushort NodeID, ushort SlotID, ref uint MC_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_motion_status_multi_axes")]
        public static extern short CS_DMC_01_motion_status_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref uint MC_status);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_interrutp_buffer")]
        public static extern short CS_DMC_01_set_interrutp_buffer(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_command")]
        public static extern short CS_DMC_01_get_command(ushort CardNo, ushort NodeID, ushort SlotID, ref int cmd);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_command_multi_axes")]
        public static extern short CS_DMC_01_get_command_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int cmd);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_target_pos")]
        public static extern short CS_DMC_01_get_target_pos(ushort CardNo, ushort NodeID, ushort SlotID, ref int pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_target_pos_multi_axes")]
        public static extern short CS_DMC_01_get_target_pos_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int pos);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_target_pos")]
        public static extern short CS_DMC_01_set_target_pos(ushort CardNo, ushort NodeID, ushort SlotID, int pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_current_speed")]
        public static extern short CS_DMC_01_get_current_speed(ushort CardNo, ushort NodeID, ushort SlotID, ref int speed);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_current_speed_multi_axes")]
        public static extern short CS_DMC_01_get_current_speed_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int speed);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_position")]
        public static extern ushort CS_DMC_01_set_position(ushort CardNo, ushort NodeID, ushort SlotID, int pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_command")]
        public static extern short CS_DMC_01_set_command(ushort CardNo, ushort NodeID, ushort SlotID, int cmd);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_position")]
        public static extern short CS_DMC_01_get_position(ushort CardNo, ushort NodeID, ushort SlotID, ref int pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_position_multi_axes")]
        public static extern short CS_DMC_01_get_position_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int pos);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_current_speed_rpm")]
        public static extern short CS_DMC_01_get_current_speed_rpm(ushort CardNo, ushort NodeID, ushort SlotID, ref int rpm);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_current_speed_rpm_multi_axes")]
        public static extern short CS_DMC_01_get_current_speed_rpm_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int rpm);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_buffer_length")]
        public static extern short CS_DMC_01_get_buffer_length(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort bufferLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_buffer_length_multi_axes")]
        public static extern short CS_DMC_01_get_buffer_length_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref ushort bufferLength);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_gear")]
        public static extern short CS_DMC_01_set_gear(ushort CardNo, ushort NodeID, ushort SlotID, short numerator, short denominator, ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_gear2")]
        public static extern short CS_DMC_01_set_gear2(ushort CardNo, ushort NodeID, ushort SlotID, double numerator, double denominator, ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_torque_mode")]
        public static extern short CS_DMC_01_set_torque_mode(ushort CardNo, ushort NodeID, ushort SlotID, uint slope);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_torque")]
        public static extern short CS_DMC_01_set_torque(ushort CardNo, ushort NodeID, ushort SlotID, int ratio);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_torque_stop")]
        public static extern short CS_DMC_01_set_torque_stop(ushort CardNo, ushort NodeID, ushort SlotID, ushort stop);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_torque_velocity_feed_forward")]
        public static extern short CS_DMC_01_set_torque_velocity_feed_forward(ushort CardNo, ushort NodeID, ushort SlotID, uint velocity_feed_forward);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_torque_velocity_limit")]
        public static extern short CS_DMC_01_set_torque_velocity_limit(ushort CardNo, ushort NodeID, ushort SlotID, uint velocity_limit);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_velocity_mode")]
        public static extern short CS_DMC_01_set_velocity_mode(ushort CardNo, ushort NodeID, ushort SlotID, double Tacc, double Tdec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_velocity")]
        public static extern short CS_DMC_01_set_velocity(ushort CardNo, ushort NodeID, ushort SlotID, int rpm);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_velocity_stop")]
        public static extern short CS_DMC_01_set_velocity_stop(ushort CardNo, ushort NodeID, ushort SlotID, ushort stop);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_velocity_torque_feed_forward")]
        public static extern short CS_DMC_01_set_velocity_torque_feed_forward(ushort CardNo, ushort NodeID, ushort SlotID, uint torque_feed_forward);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_velocity_torque_limit")]
        public static extern short CS_DMC_01_set_velocity_torque_limit(ushort CardNo, ushort NodeID, ushort SlotID, uint torque_limit);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_torque")]
        public static extern short CS_DMC_01_get_torque(ushort CardNo, ushort NodeID, ushort SlotID, ref short torque);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_torque_multi_axes")]
        public static extern short CS_DMC_01_get_torque_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref short torque);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rpm")]
        public static extern short CS_DMC_01_get_rpm(ushort CardNo, ushort NodeID, ushort SlotID, ref short rpm);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_servo_parameter")]
        public static extern short CS_DMC_01_read_servo_parameter(ushort CardNo, ushort NodeID, ushort SlotID, ushort group, ushort idx, ref uint data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_write_servo_parameter")]
        public static extern short CS_DMC_01_write_servo_parameter(ushort CardNo, ushort NodeID, ushort SlotID, ushort group, ushort idx, uint data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_ralm")]
        public static extern short CS_DMC_01_set_ralm(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_ralm_servo_all")]
        public static extern short CS_DMC_01_set_ralm_servo_all(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_alm_code")]
        public static extern short CS_DMC_01_get_alm_code(ushort CardNo, ushort NodeID, ushort SlotID, ref uint alm_code);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_master_alm_code")]
        public static extern short CS_DMC_01_master_alm_code(ushort CardNo, ref ushort alm_code);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_slave_error")]
        public static extern short CS_DMC_01_slave_error(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort alm_cnt);

        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_app_get_circle_endpoint")]
        public static extern short CS_misc_app_get_circle_endpoint(int Start_X, int Start_Y, int Center_X, int Center_Y, double Angle, ref int end_x, ref int end_y);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_app_get_circle_center_point")]
        public static extern short CS_misc_app_get_circle_center_point(int Start_X, int Start_Y, int End_x, int End_y, double Angle, ref int Center_X, ref int Center_Y);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_set_record_debuging")]
        public static extern short CS_misc_set_record_debuging(ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_open_record_debuging_file")]
        public static extern short CS_misc_open_record_debuging_file(ushort CardNo, string file_name, ushort open);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_security")]
        public static extern short CS_misc_security(uint OtherWord0, uint OtherWord1, uint SyntekWord0, uint SyntekWord1, ref uint Password0, ref uint Password1, ref uint Password2, ref uint Password3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_get_serialno")]
        public static extern short CS_misc_slave_get_serialno(ushort CardNo, ushort NodeID, ushort SlotID, ref uint SerialNO);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_check_verifykey")]
        public static extern short CS_misc_slave_check_verifykey(ushort CardNo, ushort NodeID, ushort SlotID, ref uint VerifyKey, ref ushort Lock_state);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_check_userpassword")]
        public static extern short CS_misc_slave_check_userpassword(ushort CardNo, ushort NodeID, ushort SlotID, ref uint Password_data, ref ushort Password_state);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_write_userpassword")]
        public static extern short CS_misc_slave_write_userpassword(ushort CardNo, ushort NodeID, ushort SlotID, ref uint Password_data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_write_verifykey")]
        public static extern short CS_misc_slave_write_verifykey(ushort CardNo, ushort NodeID, ushort SlotID, ref uint VerifyKey);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_user_data_buffer_write")]
        public static extern short CS_misc_slave_user_data_buffer_write(ushort CardNo, ushort NodeID, ushort SlotID, ushort Address, ref uint Data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_user_data_to_flash")]
        public static extern short CS_misc_slave_user_data_to_flash(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_slave_user_data_buffer_read")]
        public static extern short CS_misc_slave_user_data_buffer_read(ushort CardNo, ushort NodeID, ushort SlotID, ushort Address, ref uint Data);
        //I16 _stdcall _misc_app_get_spiral_endpoint(I32 start_X, I32 start_Y, I32 center_X, I32 center_Y, I32 spiral_interval, I32 spiral_angle, I32* end_x, I32* end_y);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_input_value")]
        public static extern short CS_DMC_01_get_rm_input_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_input_value_multi_axes")]
        public static extern short CS_DMC_01_get_rm_input_value_multi_axes(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort port0, ref ushort port1, ref ushort port2, ref ushort port3);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_input_filter")]
        public static extern short CS_DMC_01_set_rm_input_filter(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_input_filter_enable")]
        public static extern short CS_DMC_01_set_rm_input_filter_enable(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_output_value")]
        public static extern short CS_DMC_01_set_rm_output_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_output_value")]
        public static extern short CS_DMC_01_get_rm_output_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_output_value_error_handle")]
        public static extern short CS_DMC_01_set_rm_output_value_error_handle(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_output_value_error_handle")]
        public static extern short CS_DMC_01_get_rm_output_value_error_handle(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_output_active")]
        public static extern short CS_DMC_01_set_rm_output_active(ushort CardNo, ushort NodeID, ushort SlotID, ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_mpg_axes_enable")]
        public static extern short CS_DMC_01_set_rm_mpg_axes_enable(ushort CardNo, ushort MasterNodeID, ushort MasterSlotID, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort pulser_ratio, ref uint ratio, ref uint slop);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_mpg_axes_enable2")]
        public static extern short CS_DMC_01_set_rm_mpg_axes_enable2(ushort CardNo, ushort MasterNodeID, ushort MasterSlotID, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort pulser_ratio, ref uint ratio, ref uint slop, ref ushort denominator);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_jog_axes_enable")]
        public static extern short CS_DMC_01_set_rm_jog_axes_enable(ushort CardNo, ushort MasterNodeID, ushort MasterSlotID, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort jog_mode, ref int jog_speed, ref double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_command_buf_clear")]
        public static extern short CS_DMC_01_command_buf_clear(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_buf_dwell")]
        public static extern short CS_DMC_01_buf_dwell(short CardNo, ushort NodeID, ushort SlotID, int dwell_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sync_move")]
        public static extern short CS_DMC_01_sync_move(short CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sync_move_config")]
        public static extern short CS_DMC_01_sync_move_config(short CardNo, ushort NodeID, ushort SlotID, short enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_soft_limit")]
        public static extern short CS_DMC_01_set_soft_limit(ushort CardNo, ushort NodeID, ushort SlotID, int PLimit, int NLimit);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_soft_limit")]
        public static extern short CS_DMC_01_enable_soft_limit(ushort CardNo, ushort NodeID, ushort SlotID, short Action);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_disable_soft_limit")]
        public static extern short CS_DMC_01_disable_soft_limit(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_soft_limit_status")]
        public static extern short CS_DMC_01_get_soft_limit_status(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort PLimit_sts, ref ushort NLimit_sts);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_slave_version")]
        public static extern short CS_DMC_01_get_slave_version(short CardNo, ushort NodeID, ushort SlotID, ref ushort version);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_slave_subversion")]
        public static extern short CS_DMC_01_get_slave_subversion(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort SubVersion);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_monitor")]
        public static extern short CS_DMC_01_set_monitor(ushort CardNo, ushort NodeID, ushort SlotID, ushort monitorw);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_monitor")]
        public static extern short CS_DMC_01_get_monitor(ushort CardNo, ushort NodeID, ushort SlotID, ref uint value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_servo_command")]
        public static extern short CS_DMC_01_get_servo_command(ushort CardNo, ushort NodeID, ushort SlotID, ref int servo_cmd);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_servo_DI")]
        public static extern short CS_DMC_01_get_servo_DI(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort servo_DI);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_servo_DO")]
        public static extern short CS_DMC_01_get_servo_DO(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort servo_DO);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_trigger_buf_function")]
        public static extern short CS_DMC_01_set_trigger_buf_function(short CardNo, ushort NodeID, ushort SlotID, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_multi_axes_move")]
        public static extern short CS_DMC_01_multi_axes_move(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int DistArrary, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_spiral_xy")]
        public static extern short CS_DMC_01_start_spiral_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int spiral_interval, int spiral_angle, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_spiral2_xy")]
        public static extern short CS_DMC_01_start_spiral2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int center_x, int center_y, int end_x, int end_y, ushort dir, ushort circlenum, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_ipulser_mode")]
        public static extern short CS_DMC_01_set_rm_04pi_ipulser_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_opulser_mode")]
        public static extern short CS_DMC_01_set_rm_04pi_opulser_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_svon_polarity")]
        public static extern short CS_DMC_01_set_rm_04pi_svon_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ushort polarity);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_DO2")]
        public static extern short CS_DMC_01_set_rm_04pi_DO2(ushort CardNo, ushort NodeID, ushort SlotID, ushort ON_OFF);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_04pi_set_poweron")]
        public static extern short CS_DMC_01_04pi_set_poweron(ushort CardNo, ushort NodeID, ushort SlotID, ushort ON_OFF);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_homing_ratio")]
        public static extern short CS_DMC_01_set_rm_04pi_homing_ratio(ushort CardNo, ushort NodeID, ushort SlotID, ushort ratio);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_get_MEL_polarity")]
        public static extern short CS_DMC_01_rm_04pi_get_MEL_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_set_MEL_polarity")]
        public static extern short CS_DMC_01_rm_04pi_set_MEL_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ushort inverse);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_get_PEL_polarity")]
        public static extern short CS_DMC_01_rm_04pi_get_PEL_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_set_PEL_polarity")]
        public static extern short CS_DMC_01_rm_04pi_set_PEL_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ushort inverse);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_move")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_v_move")]
        public static extern short CS_DMC_01_rm_04pi_md1_v_move(ushort CardNo, ushort NodeID, ushort SlotID, int StrVel, int MaxVel, double Tacc, short dir, ushort m_curve);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_line2")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_line2(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Dist, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_arc")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_arc(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Center, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_line3")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_line3(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Dist, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_line4")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_line4(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Dist, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_arc2")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_arc2(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int End, double Angle, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_arc3")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_arc3(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Center, ref int End, short dir, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_start_heli")]
        public static extern short CS_DMC_01_rm_04pi_md1_start_heli(ushort CardNo, ushort NodeID, ref ushort SlotID, ref int Center, int Depth, int Pitch, short dir, int StrVel, int MaxVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_set_gear")]
        public static extern short CS_DMC_01_rm_04pi_md1_set_gear(ushort CardNo, ushort NodeID, ushort SlotID, short numerator, short denominator, ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_p_change")]
        public static extern short CS_DMC_01_rm_04pi_md1_p_change(ushort CardNo, ushort NodeID, ushort SlotID, int NewPos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_v_change")]
        public static extern short CS_DMC_01_rm_04pi_md1_v_change(ushort CardNo, ushort NodeID, ushort SlotID, int NewSpeed, double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_set_soft_limit")]
        public static extern short CS_DMC_01_rm_04pi_md1_set_soft_limit(ushort CardNo, ushort NodeID, ushort SlotID, int PLimit, int NLimit, ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_get_soft_limit_status")]
        public static extern short CS_DMC_01_rm_04pi_md1_get_soft_limit_status(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort NLimit_status, ref ushort PLimit_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_set_sld")]
        public static extern short CS_DMC_01_rm_04pi_md1_set_sld(ushort CardNo, ushort NodeID, ushort SlotID, short enable, short sd_logic, short mode);
        //I16 _stdcall _DMC_01_04pi_md1_get_mc_error_code(U16 CardNo, U16 NodeID, U16 SlotID, U16 *error_code);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_ref_counter")]
        public static extern short CS_DMC_01_set_rm_04pi_ref_counter(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04PI_get_buffer")]
        public static extern short CS_DMC_01_rm_04PI_get_buffer(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort bufferLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04pi_md1_get_mc_error_code")]
        public static extern short CS_DMC_01_rm_04pi_md1_get_mc_error_code(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort error_code);

 
        public delegate void Delegate_Callback(ushort CardNo, ushort node);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_link_interrupt")]
        public static extern short CS_DMC_01_link_interrupt(ushort nCardNo, Delegate_Callback Callback);
        
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_int_factor")]
        public static extern short CS_DMC_01_set_int_factor(ushort CardNo, ushort NodeID, ushort int_factor);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_int_enable")]
        public static extern short CS_DMC_01_int_enable(ushort CardNo, ushort NodeID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_int_count")]
        public static extern short CS_DMC_01_get_int_count(ushort CardNo, ushort NodeID, ref ushort count);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_int_disable")]
        public static extern short CS_DMC_01_int_disable(ushort CardNo, ushort NodeID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_int_status")]
        public static extern short CS_DMC_01_get_int_status(ushort CardNo, ushort NodeID, ref ushort event_int_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_security")]
        public static extern short CS_DMC_01_read_security(ushort CardNo, ushort page, ref ushort array);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_security_status")]
        public static extern short CS_DMC_01_read_security_status(ushort CardNo, ref ushort status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_write_security_status")]
        public static extern short CS_DMC_01_write_security_status(ushort CardNo, ushort status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_write_security")]
        public static extern short CS_DMC_01_write_security(ushort CardNo, ushort page, ref ushort array);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_check_userpassword")]
        public static extern short CS_DMC_01_check_userpassword(ushort CardNo, ref uint password_data, ref ushort password_state);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_write_userpassword")]
        public static extern short CS_DMC_01_write_userpassword(ushort CardNo, ref uint password_data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_check_verifykey")]
        public static extern short CS_DMC_01_check_verifykey(ushort CardNo, ref uint VerifyKey, ref ushort state);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_write_verifykey")]
        public static extern short CS_DMC_01_write_verifykey(ushort CardNo, ref uint verifykey);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_read_serialno")]
        public static extern short CS_DMC_01_read_serialno(ushort CardNo, ref uint SerialNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_value")]
        public static extern short CS_DMC_01_rm_04da_set_output_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_get_return_code")]
        public static extern short CS_DMC_01_rm_04da_get_return_code(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort returncode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_range")]
        public static extern short CS_DMC_01_rm_04da_set_output_range(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort range);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_enable")]
        public static extern short CS_DMC_01_rm_04da_set_output_enable(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_overrange")]
        public static extern short CS_DMC_01_rm_04da_set_output_overrange(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_error_clear")]
        public static extern short CS_DMC_01_rm_04da_set_output_error_clear(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_read_control_register")]
        public static extern short CS_DMC_01_rm_04da_read_control_register(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort control_register_data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_get_output_value")]
        public static extern short CS_DMC_01_rm_04da_get_output_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_error_handle")]
        public static extern short CS_DMC_01_rm_04da_set_output_error_handle(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_set_output_offset_value")]
        public static extern short CS_DMC_01_rm_04da_set_output_offset_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, short value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_get_output_offset_value")]
        public static extern short CS_DMC_01_rm_04da_get_output_offset_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref short value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_rm_04da_read_data")]
        public static extern short CS_DMC_01_rm_04da_read_data(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_04ad_input_range")]
        public static extern short CS_DMC_01_set_04ad_input_range(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort range);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04ad_input_range")]
        public static extern short CS_DMC_01_get_04ad_input_range(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort range);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_04ad_conversion_time")]
        public static extern short CS_DMC_01_set_04ad_conversion_time(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04ad_conversion_time")]
        public static extern short CS_DMC_01_get_04ad_conversion_time(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04ad_data")]
        public static extern short CS_DMC_01_get_04ad_data(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04ad_data_multi_axes")]
        public static extern short CS_DMC_01_get_04ad_data_multi_axes(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort value0, ref ushort value1, ref ushort value2, ref ushort value3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04da_Output_range")]
        public static extern short CS_DMC_01_get_04da_Output_range(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort range);


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_04ad_average_mode")]
        public static extern short CS_DMC_01_set_04ad_average_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_04ad_average_mode")]
        public static extern short CS_DMC_01_get_04ad_average_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ref ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_04ad_input_enable")]
        public static extern short CS_DMC_01_set_04ad_input_enable(ushort CardNo, ushort NodeID, ushort SlotID, ushort channelno, ushort ON_OFF);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_group")]
        public static extern short CS_DMC_01_set_group(ushort CardNo, ref ushort NodeID, ref ushort SlotID, ushort NodeID_Num, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_speed_continue")]
        public static extern short CS_DMC_01_speed_continue(ushort CardNo, ushort NodeID, ushort SlotID, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_speed_continue_mode")]
        public static extern short CS_DMC_01_speed_continue_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_speed_continue_combine_ratio")]
        public static extern short CS_DMC_01_speed_continue_combine_ratio(ushort CardNo, ushort NodeID, ushort SlotID, ushort ratio);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_scurve_rate")]
        public static extern short CS_DMC_01_set_scurve_rate(ushort CardNo, ushort NodeID, ushort SlotID, ushort scurve_rate);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_dda_mode")]
        public static extern short CS_DMC_01_enable_dda_mode(ushort CardNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dda_data")]
        public static extern short CS_DMC_01_set_dda_data(ushort CardNo, ref int abs_pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dda_cnt")]
        public static extern short CS_DMC_01_get_dda_cnt(ushort CardNo, ref ushort dda_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_feedrate_overwrite")]
        public static extern short CS_DMC_01_feedrate_overwrite(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode, int New_Speed, double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_move")]
        public static extern short CS_DMC_01_start_v3_move(ushort CardNo, ushort NodeID, ushort SlotID, int Dist, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_move_xy")]
        public static extern short CS_DMC_01_start_v3_move_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_arc_xy")]
        public static extern short CS_DMC_01_start_v3_arc_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_arc2_xy")]
        public static extern short CS_DMC_01_start_v3_arc2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int End_X, int End_Y, double Angle, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_arc3_xy")]
        public static extern short CS_DMC_01_start_v3_arc3_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int End_x, int End_y, short Dir, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_move_xyz")]
        public static extern short CS_DMC_01_start_v3_move_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int DisX, int DisY, int DisZ, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_heli_xy")]
        public static extern short CS_DMC_01_start_v3_heli_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int Depth, int Pitch, short Dir, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_multi_axes")]
        public static extern short CS_DMC_01_start_v3_multi_axes(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ref int DistArrary, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_spiral_xy")]
        public static extern short CS_DMC_01_start_v3_spiral_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, int spiral_interval, uint spiral_angle, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_spiral2_xy")]
        public static extern short CS_DMC_01_start_v3_spiral2_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int center_x, int center_y, int end_x, int end_y, ushort dir, ushort circlenum, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_fifo_cnt")]
        public static extern short CS_DMC_01_get_fifo_cnt(ushort CardNo, ref ushort fifo_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_liner_speed_master")]
        public static extern short CS_DMC_01_liner_speed_master(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_record_mode")]
        public static extern short CS_DMC_01_enable_record_mode(ushort CardNo, ushort enable, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_record_data")]
        public static extern short CS_DMC_01_get_record_data(ushort CardNo, ref uint record_data);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dec_acceleration")]
        public static extern short CS_DMC_01_set_dec_acceleration(ushort CardNo, ushort NodeID, ushort SlotID, double pss);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_sd_time")]
        public static extern short CS_DMC_01_set_sd_time(ushort CardNo, ushort NodeID, ushort SlotID, double sd_dec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_DLL_path")]
        public static extern short CS_DMC_01_get_DLL_path(StringBuilder lpFilePath, uint nSize, ref uint nLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_DLL_version")]
        public static extern short CS_DMC_01_get_DLL_version(StringBuilder lpBuf, uint nSize, ref uint nLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_DLL_path_Single")]
        public static extern short CS_DMC_01_get_DLL_path_Single(ushort CardNo, StringBuilder lpFilePath, uint nSize, ref uint nLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_DLL_version_Single")]
        public static extern short CS_DMC_01_get_DLL_version_Single(ushort CardNo, StringBuilder lpBuf, uint nSize, ref uint nLength);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_switch_buffer")]
        public static extern short CS_DMC_01_enable_switch_buffer(ushort CardNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_save_device_info")]
        public static extern short CS_DMC_01_save_device_info(ushort CardNo, string path);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_compare_device_info")]
        public static extern short CS_DMC_01_compare_device_info(ushort CardNo, string path, string pathLog);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_sld_stop")]
        public static extern short CS_DMC_01_set_sld_stop(ushort CardNo, ushort NodeID, ushort SlotID, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_sld_mode")]
        public static extern short CS_DMC_01_set_sld_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_pdo_Rx_error")]
        public static extern short CS_DMC_01_pdo_Rx_error(ushort CardNo, ushort NodeID, ushort SlotID, ref uint Rx_error1, ref uint Rx_error2, ushort set_get);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_pdo_Tx_error")]
        public static extern short CS_DMC_01_pdo_Tx_error(ushort CardNo, ushort NodeID, ushort SlotID, ref uint Tx_error, ushort set_get);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_sphere_xyz")]
        public static extern short CS_DMC_01_start_v3_sphere_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int pos1_x, int pos1_y, int pos1_z, int pos2_x, int pos2_y, int pos2_z, int StrVel, int MaxVel, int EndVel, double Tacc, double Tdec, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_function_time")]
        public static extern short CS_DMC_01_set_function_time(ushort CardNo, uint ms1000);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_mc_fifo_mode")]
        public static extern short CS_DMC_01_set_mc_fifo_mode(ushort CardNo, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_axis_mode")]
        public static extern short CS_DMC_01_enable_axis_mode(ushort CardNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_clear_dda_data")]
        public static extern short CS_DMC_01_clear_dda_data(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_pitch")]
        public static extern short CS_DMC_01_set_cam_pitch(ushort CardNo, ushort NodeID, ushort SlotID, int pitch);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_org")]
        public static extern short CS_DMC_01_set_cam_org(short CardNo, ushort NodeID, ushort SlotID, short Dir, int orgpos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_enable")]
        public static extern short CS_DMC_01_set_cam_enable(short CardNo, ushort NodeID, ushort SlotID, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_mode")]
        public static extern short CS_DMC_01_set_cam_mode(short CardNo, ushort NodeID, ushort SlotID, ushort Mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_table")]
        public static extern short CS_DMC_01_set_cam_table(short CardNo, ushort NodeID, ushort SlotID, short Dir, ref int table);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cam_table2")]
        public static extern short CS_DMC_01_set_cam_table2(short CardNo, ushort NodeID, ushort SlotID, short Dir, ref int table, int Num);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dmc_mode")]
        public static extern short CS_DMC_01_set_dmc_mode(short CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_backlash")]
        public static extern short CS_DMC_01_set_backlash(short CardNo, ushort NodeID, ushort SlotID, short enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_backlash_info")]
        public static extern short CS_DMC_01_set_backlash_info(short CardNo, ushort NodeID, ushort SlotID, short backlash, ushort accstep, ushort contstep, ushort decstep);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_electcam")]
        public static extern short CS_DMC_01_enable_electcam(short CardNo, ushort NodeID, ushort SlotID, ushort enable, ushort axisbit, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_cv_mode")]
        public static extern short CS_DMC_01_set_cv_mode(short CardNo, ushort NodeID, ushort SlotID, ushort flag_const_v, ushort flag_dec_c);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_ex_position")]
        public static extern short CS_DMC_01_enable_ex_position(short CardNo, ushort NodeID, ushort SlotID, ushort enable);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_always_monitorid")]
        public static extern short CS_DMC_01_enable_always_monitorid(ushort CardNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_nrline_status")]
        public static extern short CS_DMC_01_get_nrline_status(short CardNo, ushort NodeID, ushort SlotID, ref ushort nrline_sts);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_gear")]
        public static extern short CS_DMC_01_get_gear(ushort CardNo, ushort NodeID, ushort SlotID, ref short numerator, ref short denominator, ref ushort Enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_01ph_alm_polarity")]
        public static extern short CS_DMC_01_set_01ph_alm_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ushort polarity);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_01ph_org_polarity")]
        public static extern short CS_DMC_01_set_01ph_org_polarity(ushort CardNo, ushort NodeID, ushort SlotID, ushort polarity);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_output_value_HPF")]
        public static extern short CS_DMC_01_get_rm_output_value_HPF(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_output_value_HPF")]
        public static extern short CS_DMC_01_set_rm_output_value_HPF(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort value);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_position")]
        public static extern short CS_DMC_01_set_compare_channel_position(ushort CardNo, ushort compare_channel, int position);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_compare_channel_position")]
        public static extern short CS_DMC_01_get_compare_channel_position(ushort CardNo, ushort compare_channel, ref int position);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_ipulser_mode")]
        public static extern short CS_DMC_01_set_compare_ipulser_mode(ushort CardNo, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_direction")]
        public static extern short CS_DMC_01_set_compare_channel_direction(ushort CardNo, ushort compare_channel, ushort dir);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_trigger_time")]
        public static extern short CS_DMC_01_set_compare_channel_trigger_time(ushort CardNo, ushort compare_channel, uint time_us);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_one_shot")]
        public static extern short CS_DMC_01_set_compare_channel_one_shot(ushort CardNo, ushort compare_channel);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_source")]
        public static extern short CS_DMC_01_set_compare_channel_source(ushort CardNo, ushort compare_channel, ushort source);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel0_position_cmp")]
        public static extern short CS_DMC_01_channel0_position_cmp(ushort CardNo, int start, ushort dir, ushort interval, uint trigger_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel0_position_cmp_by_gpio")]
        public static extern short CS_DMC_01_channel0_position_cmp_by_gpio(ushort CardNo, ushort dir, ushort interval, int trigger_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_output_enable")]
        public static extern short CS_DMC_01_channel1_output_enable(ushort CardNo, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_output_mode")]
        public static extern short CS_DMC_01_channel1_output_mode(ushort CardNo, ushort mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_get_io_status")]
        public static extern short CS_DMC_01_channel1_get_io_status(ushort CardNo, ref ushort io_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_set_gpio_out")]
        public static extern short CS_DMC_01_channel1_set_gpio_out(ushort CardNo, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_get_fifo_status")]
        public static extern short CS_DMC_01_channel1_get_fifo_status(ushort CardNo, ref ushort fifo_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_get_fifo_counter")]
        public static extern short CS_DMC_01_channel1_get_fifo_counter(ushort CardNo, ref ushort fifo_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_position_compare_table")]
        public static extern short CS_DMC_01_channel1_position_compare_table(ushort CardNo, ref int pos_table, uint table_size);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_position_compare_table_level")]
        public static extern short CS_DMC_01_channel1_position_compare_table_level(ushort CardNo, ref int pos_table, ref uint level_table, uint table_size);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_position_compare_table_cnt")]
        public static extern short CS_DMC_01_channel1_position_compare_table_cnt(ushort CardNo, ref uint cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_polarity")]
        public static extern short CS_DMC_01_set_compare_channel_polarity(ushort CardNo, ushort inverse);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_position_re_compare_table")]
        public static extern short CS_DMC_01_channel1_position_re_compare_table(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_channel1_position_re_compare_table_level")]
        public static extern short CS_DMC_01_channel1_position_re_compare_table_level(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_compare_channel_enable")]
        public static extern short CS_DMC_01_set_compare_channel_enable(ushort CardNo, ushort compare_channel, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_compare_channel_direction")]
        public static extern short CS_DMC_01_get_compare_channel_direction(ushort CardNo, ushort compare_channel, ref ushort dir);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_feedrate_overwrite")]
        public static extern short CS_DMC_01_get_feedrate_overwrite(ushort CardNo, ushort NodeID, ushort SlotID, ref int New_Speed);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_v_change")]
        public static extern short CS_DMC_01_get_v_change(ushort CardNo, ushort NodeID, ushort SlotID, ref int New_Speed);

        //2013.10.23
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_position_ref_source")]
        public static extern short CS_DMC_01_set_position_ref_source(short CardNo, ushort NodeID, ushort SlotID, short ref_source);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_position_ref_source")]
        public static extern short CS_DMC_01_get_position_ref_source(short CardNo, ushort NodeID, ushort SlotID, ref short ref_source);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_virtual_command")]
        public static extern short CS_DMC_01_set_virtual_command(short CardNo, ushort NodeID, ushort SlotID, int virtual_command);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_virtual_command")]
        public static extern short CS_DMC_01_get_virtual_command(short CardNo, ushort NodeID, ushort SlotID, ref int virtual_command);

        //2013.11.07
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_virtual_position")]
        public static extern short CS_DMC_01_set_virtual_position(short CardNo, ushort NodeID, ushort SlotID, int virtual_Position);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_disable_virtual_pos")]
        public static extern short CS_DMC_01_disable_virtual_pos(short CardNo, ushort NodeID, ushort SlotID, ushort on_off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_Stepping_Move_rate")]
        public static extern short CS_DMC_01_Set_Stepping_Move_rate(short CardNo, ushort NodeID, ushort SlotID, byte MoveRatio);
        //04-PI mode1: Speed will reduce cw/ccw Max Speed 200k/mratio,AB phase 500k/mratio,MoveRatio = 1~255
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_Stepping_Move_rate")]
        public static extern short CS_DMC_01_Get_Stepping_Move_rate(short CardNo, ushort NodeID, ushort SlotID, ref byte MoveRatio);

        //2013.11.14
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_pdo_mode")]
        public static extern short CS_DMC_01_set_pdo_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort Enable);

        //2013.11.27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_mc_buffer")]
        public static extern short CS_DMC_01_set_mc_buffer(ushort CardNo, ushort status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_multi_axes_arc")]
        public static extern short CS_DMC_01_start_v3_multi_axes_arc(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_feedrate_overwrite_rate")]
        public static extern short CS_DMC_01_set_feedrate_overwrite_rate(short CardNo, ushort NodeID, ushort SlotID, double feedrate);

        //2013.12.31
        [DllImport("PCI_DMC.dll", EntryPoint = "_misc_app_get_ellipse_para")]
        public static extern short CS_misc_app_get_ellipse_para(double x0, double y0, double x1, double y1, double x2, double y2, ref double center_x, ref double center_y, ref double radius);

        //20140120
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_acc_dec_2_slop")]
        public static extern short CS_DMC_01_set_acc_dec_2_slop(ushort CardNo, ushort time_slop_enable);

        //20140127
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MRAM_read_word")]
        public static extern short CS_DMC_01_MRAM_read_word(uint offset, ref ushort word_value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MRAM_write_word")]
        public static extern short CS_DMC_01_MRAM_write_word(uint offset, ushort word_value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MRAM_read_dword")]
        public static extern short CS_DMC_01_MRAM_read_dword(uint offset, ref uint dword_value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MRAM_write_dword")]
        public static extern short CS_DMC_01_MRAM_write_dword(uint offset, uint dword_value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_pr_mode")]
        public static extern short CS_DMC_01_set_pr_mode(ushort CardNo, ushort NodeID, ushort SlotID);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_pdo_write_servo_parameter")]
        public static extern short CS_DMC_01_pdo_write_servo_parameter(ushort CardNo, ushort NodeID, ushort SlotID, ushort Group, ushort Index, uint Data);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_LED_Switch")]
        public static extern byte CS_DMC_01_LED_Switch(byte LedNum, byte Switch);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_ecam_repeat")]
        public static extern short CS_DMC_01_set_ecam_repeat(ushort CardNo, ushort NodeID, ushort SlotID, int start_pos, int end_pos, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_limit_auto_ralm")]
        public static extern short CS_DMC_01_set_limit_auto_ralm(ushort CardNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_APICheck")]
        public static extern short CS_DMC_01_Set_APICheck(ushort CardNo, ushort Mode);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_msbrline_move")]
        public static extern short CS_DMC_01_start_v3_msbrline_move(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, ushort ArcNodeBit, ref int DistArray, ref int DistArray2, ushort Mode, double Parameter, double SpdRatio, double Angle1, double Angle2, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_r_a);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_01_ph_inposition")]
        public static extern short CS_DMC_01_set_01_ph_inposition(ushort CardNo, ushort NodeID, ushort SlotID, ushort poswindow_time, uint poswindow);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_01_ph_inposition")]
        public static extern short CS_DMC_01_get_01_ph_inposition(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort poswindow_time, ref uint poswindow);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dsp_intext4cnt")]
        public static extern short CS_DMC_01_get_dsp_intext4cnt(ushort CardNo, ref uint tm0, ref uint tm1, ref uint tm2, ref uint tm3);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_pdo_check_servo_status")]
        public static extern short CS_DMC_01_pdo_check_servo_status(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort Status);
        //20140418
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_node_actmotion")]
        public static extern short CS_DMC_01_set_node_actmotion(ushort CardNo, ref ushort NodeID, ushort num);
        //20140505
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_module_mode")]
        public static extern short CS_DMC_01_get_module_mode(ushort CardNo, ushort NodeID, ushort SlotID, ref uint MC_Mode);
        //20140512
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_ref_counter")]
        public static extern short CS_DMC_01_set_ref_counter(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);

        //20140530
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_multi_table_V3_move")]
        public static extern short CS_DMC_01_multi_table_V3_move(ushort CardNo, ushort MotionNum, ushort NodeID, ushort SlotID, ref int Dist, int StrVel, ref int ConstVel, int EndVel, ref double Tphase, double Tdec, ushort m_curve, ushort m_r_a);

        //20140709
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_record_cnt")]
        public static extern short CS_DMC_01_get_record_cnt(ushort CardNo, ref ushort Record_Cnt);

        //20141205
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_watchdog_cnt")]
        public static extern short CS_DMC_01_get_watchdog_cnt(ushort CardNo, ref uint watchdog_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_watchdog_cnt")]
        public static extern short CS_DMC_01_set_watchdog_cnt(ushort CardNo, uint watchdog_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_watchdog_value")]
        public static extern short CS_DMC_01_get_watchdog_value(ushort CardNo, ref uint watchdog_value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_watchdog_value")]
        public static extern short CS_DMC_01_set_watchdog_value(ushort CardNo, uint watchdog_value);

        //20141215
        [DllImport("PCI_DMC.dll", EntryPoint = "DMCSMBox_Check")]
        public static extern ushort CSDMCSMBox_Check(string fmt);

        //20150120
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_md2_feedback_mode")]
        public static extern short CS_DMC_01_set_rm_04pi_md2_feedback_mode(ushort CardNo, ushort NodeID, ushort SlotID, ushort mode);

        //20150407
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_home_config_Tdec")]
        public static extern short CS_DMC_01_set_rm_04pi_home_config_Tdec(ushort CardNo, ushort NodeID, ushort SlotID, uint Tdec, uint enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_04pi_Home_counter_Delay_time")]
        public static extern short CS_DMC_01_set_rm_04pi_Home_counter_Delay_time(ushort CardNo, ushort NodeID, ushort SlotID, ushort Delay_time);

        //20150522
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_04pi_home_config_Tdec")]
        public static extern short CS_DMC_01_get_rm_04pi_home_config_Tdec(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort Tdec, ref ushort enable);

        //20150507
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_slave_error_value")]
        public static extern short CS_DMC_01_set_slave_error_value(ushort CardNo, ushort times);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_adjoint_status")]
        public static extern short CS_DMC_01_set_adjoint_status(ushort CardNo, uint adjoint_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_adjoint_status")]
        public static extern short CS_DMC_01_get_adjoint_status(ushort CardNo, ref uint adjoint_status);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_output_DW")]
        public static extern short CS_DMC_01_set_dio_output_DW(ushort CardNo, uint On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_output_DW")]
        public static extern short CS_DMC_01_get_dio_output_DW(ushort CardNo, ref uint On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_input_DW")]
        public static extern short CS_DMC_01_get_dio_input_DW(ushort CardNo, ref uint On_Off);
        //20160512
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_output_DW_error_handle")]
        public static extern short CS_DMC_01_set_dio_output_DW_error_handle(ushort CardNo, ushort value);
        //20160518
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_output_DW_error_handle")]
        public static extern short CS_DMC_01_get_dio_output_DW_error_handle(ushort CardNo, ref ushort value);

        //20150709
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_prov_multi_axes_arc")]
        public static extern short CS_DMC_01_start_prov_multi_axes_arc(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, double Angle, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);

        //20150710
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_last_error_node_no")]
        public static extern short CS_DMC_01_get_last_error_node_no(ushort CardNo, ref ushort ErrorNode);
        //20150803
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_rline_xy")]
        public static extern short CS_DMC_01_start_v3_rline_xy(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Target_Pos1_X, int Target_Pos1_Y, int Target_Pos2_X, int Target_Pos2_Y, ushort Mode, double Parameter, int StrVel, int ConstVel, int EndVel, double Tacc, double Tdec, ushort Curve_Mode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_rline_xyz")]
        public static extern short CS_DMC_01_start_v3_rline_xyz(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int Target_Pos1_X, int Target_Pos1_Y, int Target_Pos1_Z, int Target_Pos2_X, int Target_Pos2_Y, int Target_Pos2_Z, ushort Mode, double Parameter, int StrVel, int ConstVel, int EndVel, double Tacc, double Tdec, ushort Curve_Mode, ushort R_A_Mode);


        //2015/1014 B02
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_qep_set_mode")]
        public static extern short CS_DMC_B02_qep_set_mode(ushort nCardNo, byte nQepIndex, byte nMode);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_qep_set_inverse")]
        public static extern short CS_DMC_B02_qep_set_inverse(ushort nCardNo, byte nQepIndex, byte bEnable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_qep_get_reload_counter")]
        public static extern short CS_DMC_B02_qep_get_reload_counter(ushort nCardNo, byte nQepIndex, ref int nCounter);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_qep_set_reload_counter")]
        public static extern short CS_DMC_B02_qep_set_reload_counter(ushort nCardNo, byte nQepIndex, int nCounter);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_qep_get_counter")]
        public static extern short CS_DMC_B02_qep_get_counter(ushort nCardNo, byte nQepIndex, ref int nCounter);

        // nStatus:bit# -> Pin: 0->9   nstatus:0->關閉 /1->開啟
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_gpio_software_control_status")]
        public static extern short CS_DMC_B02_gpio_software_control_status(ushort CardNo, byte nPin, ref byte nOnOff);
        // nSetting:bit# -> Pin#: 0->9  nOnOff:0->關閉 /1->開啟
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_gpio_software_control_setting")]
        public static extern short CS_DMC_B02_gpio_software_control_setting(ushort CardNo, byte nPin, byte nOnOff);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_initial")]
        public static extern short CS_DMC_B02_ipcmp_initial(ushort nCardNo, byte nIpcmpIndex, byte nQepIndex, byte nGpioPin, byte bInverse);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_oneshot_time")]
        public static extern short CS_DMC_B02_ipcmp_oneshot_time(ushort nCardNo, byte nIpcmpIndex, ushort nPulseWidth);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_software_trigger")]
        public static extern short CS_DMC_B02_ipcmp_software_trigger(ushort nCardNo, byte nIpcmpIndex);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_compare_position")]
        public static extern short CS_DMC_B02_ipcmp_compare_position(ushort nCardNo, byte nIpcmpIndex, int nStartPosition, ushort nInterval, uint nCount, byte nDir);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_compare_trigger_count")]
        public static extern short CS_DMC_B02_ipcmp_compare_trigger_count(ushort nCardNo, byte nIpcmpIndex, ref uint nCount);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ipcmp_compare_trigger_count_clear")]
        public static extern short CS_DMC_B02_ipcmp_compare_trigger_count_clear(ushort nCardNo, byte nIpcmpIndex);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_initial")]
        public static extern short CS_DMC_B02_tpcmp_initial(ushort nCardNo, byte nFifoIndex, byte nQepIndex, byte nGpioPin, byte bInverse, byte bInterrupt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_oneshot_time")]
        public static extern short CS_DMC_B02_tpcmp_oneshot_time(ushort nCardNo, byte nFifoIndex, ushort nPulseWidth);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_software_trigger")]
        public static extern short CS_DMC_B02_tpcmp_software_trigger(ushort nCardNo, byte nFifoIndex);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_software_trigger_level")]
        public static extern short CS_DMC_B02_tpcmp_software_trigger_level(ushort nDeviceNo, byte nFifoIndex, byte bActive);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_position_table_reset")]
        public static extern short CS_DMC_B02_tpcmp_compare_position_table_reset(ushort nCardNo, byte nFifoIndex);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_position_table")]
        public static extern short CS_DMC_B02_tpcmp_compare_position_table(ushort nCardNo, byte nFifoIndex,ref int [] nPositionTable, uint nTableSize);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_position_table_level")]
        public static extern short CS_DMC_B02_tpcmp_compare_position_table_level(ushort nCardNo, byte nFifoIndex, ref int [] nPositionTable,ref byte [] nLevelTable, uint nTableSize);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_trigger_count")]
        public static extern short CS_DMC_B02_tpcmp_compare_trigger_count(ushort nCardNo, byte nFifoIndex, ref uint nCount);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_trigger_count_clear")]
        public static extern short CS_DMC_B02_tpcmp_compare_trigger_count_clear(ushort nCardNo, byte nFifoIndex);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_get_input_value")]
        public static extern short CS_DMC_B02_ext_io_get_input_value(ushort nCardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_get_output_value")]
        public static extern short CS_DMC_B02_ext_io_get_output_value(ushort nCardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_set_output_value")]
        public static extern short CS_DMC_B02_ext_io_set_output_value(ushort nCardNo, byte nState);

        //2015/10/27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_digital_filter")]
        public static extern short CS_DMC_01_set_digital_filter(ushort CardNo, ushort port, ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_digital_filter")]
        public static extern short CS_DMC_01_get_digital_filter(ushort CardNo, ushort port, ref ushort value);

        //2015/11/06
        //======LED=======
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_LED_Initial")]
        public static extern ushort CS_DMC_01_LED_Initial(ushort CardNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_LED_Signal")]
        public static extern ushort CS_DMC_01_Get_LED_Signal(ushort CardNo, ushort LedNo, ref ushort OnOff);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_LED_Signal")]
        public static extern ushort CS_DMC_01_Set_LED_Signal(ushort CardNo, ushort LedNo, ushort OnOff);
        //======GPIO=======MP1
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_GPIO_Set_Type")]
        public static extern ushort CS_DMC_01_GPIO_Set_Type(ushort CardNo, ushort PortNo, ushort Type);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_GPIO_Input_Value")]
        public static extern ushort CS_DMC_01_Get_GPIO_Input_Value(ushort CardNo, ref ushort Value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_GPIO_Output_Value")]
        public static extern ushort CS_DMC_01_Get_GPIO_Output_Value(ushort CardNo, ref ushort Value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_GPIO_Output_Value")]
        public static extern ushort CS_DMC_01_Set_GPIO_Output_Value(ushort CardNo, ushort Value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_GPIO_Output_error_handle")]
        public static extern ushort CS_DMC_01_Set_GPIO_Output_error_handle(ushort CardNo, ushort Value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_GPIO_Output_error_handle")]
        public static extern ushort CS_DMC_01_Get_GPIO_Output_error_handle(ushort CardNo, ref ushort Value);




        //2015/11/19
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_get_poisition")]
        public static extern short CS_DMC_A02_mpg_get_poisition(ushort CardNo, ref int nValue);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_fsm_get_lock_state")]
        public static extern short CS_DMC_A02_mpg_fsm_get_lock_state(ushort CardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_get_io_state_enable")]
        public static extern short CS_DMC_A02_mpg_get_io_state_enable(ushort CardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_axis_get_io_state")]
        public static extern short CS_DMC_A02_mpg_axis_get_io_state(ushort CardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_axis_get_ratio_state")]
        public static extern short CS_DMC_A02_mpg_axis_get_ratio_state(ushort CardNo, ref byte nState);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_set_mpg_axes_enable")]
        public static extern short CS_DMC_A02_set_mpg_axes_enable(ushort CardNo, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort pulser_ratio, ref uint ratio, ref uint slop, ref ushort denominator);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_set_jog_axes_enable")]
        public static extern short CS_DMC_A02_set_jog_axes_enable(ushort CardNo, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort jog_mode, ref int jog_speed, ref double sec);
        //jog_mode0->6button 有效,1->手輪選擇軸,xbutton控制jog方向
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_get_jog_io_state")]
        public static extern short CS_DMC_A02_get_jog_io_state(ushort CardNo, ref ushort nState);
        //Bit0: X+,Bit1: X-,Bit2: Y+,Bit3: Y-,Bit4: Z+,Bit5: Z-

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_clear_poisition")]
        public static extern short CS_DMC_A02_mpg_clear_poisition(ushort CardNo);

        //2017-10-02
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_set_mpg_axes_enable2")]
        public static extern short CS_DMC_A02_set_mpg_axes_enable2(ushort CardNo, ref ushort NodeID, ref ushort SlotID, ushort enable, ushort pulser_ratio, ref uint ratio, ref uint slop, ref ushort denominator);

        //2017-10-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_get_mpg_axes_enable2")]
        public static extern short CS_DMC_A02_get_mpg_axes_enable2(ushort CardNo, ref ushort Enable);

        //2017-10-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_set_mpg_axes_extend")]
        public static extern short CS_DMC_A02_set_mpg_axes_extend(ushort CardNo, ushort AxisExtend);

        //2017-10-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_get_mpg_axes_extend")]
        public static extern short CS_DMC_A02_get_mpg_axes_extend(ushort CardNo, ref ushort AxisNum);

        //2017-10-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_axis_get_io_state2")]
        public static extern short CS_DMC_A02_mpg_axis_get_io_state2(ushort CardNo, ref byte nState);
        // nState: [0]None  [1]Axis#0  [2]Axis#1  [3]Axis#2  [4]Axis#3 ...[9]Axis#10  手輪軸選擇

        //2017-10-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_A02_mpg_axis_get_ratio_state2")]
        public static extern short CS_DMC_A02_mpg_axis_get_ratio_state2(ushort CardNo, ref byte nState);
        // nState: [0]None  [1]x1  [2]x10  [3]x100   手輪倍率選擇

        //2017-11-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_MPG_AMF")]
        public static extern short CS_DMC_01_Set_MPG_AMF(short CardNo, short AMFNo, short AMFNum, ushort FilterNum, ushort Enable);

        //2017-12-01 MP1-A12D 軟體手輪 B01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_Qep_set_mpg_axis_enable")]
        public static extern short CS_DMC_Qep_set_mpg_axis_enable(ushort CardNo, ushort CompareChannel, ushort NodeID, ushort SlotID, ushort enable, ushort pulser_ratio, uint ratio, uint slop, ushort denominator);

        //2017-12-01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_Qep_get_mpg_axis_enable")]
        public static extern short CS_DMC_Qep_get_mpg_axis_enable(ushort CardNo, ref ushort CompareChannel, ref ushort NodeID, ref ushort SlotID, ref ushort enable, ref ushort pulser_ratio, ref uint ratio, ref uint slop, ref ushort denominator);

        //2017-12-01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_Qep_get_mpg_axis_cnt")]
        public static extern short CS_DMC_Qep_get_mpg_axis_cnt(ushort CardNo, ushort compare_channel, ref int cnt);

        //2017-12-01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_Qep_set_mpg_clear")]
        public static extern short CS_DMC_Qep_set_mpg_clear(ushort CardNo, ushort compare_channel);

        //20160113 B02 latch
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_latch_initial")]
        public static extern short CS_DMC_B02_ext_io_latch_initial(ushort nCardNo, byte nExternalIoNo, byte nSource, byte nFilter);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_latch_function")]
        public static extern short CS_DMC_B02_ext_io_latch_function(ushort nCardNo, byte nExternalIoNo, byte bQep0, byte bQep1, byte bQep2, byte bSw, byte bH2L, byte bL2H);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_ext_io_latch_enable")]
        public static extern short CS_DMC_B02_ext_io_latch_enable(ushort nCardNo, byte nExternalIoNo, byte bEnable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_latch_fifo_reset")]
        public static extern short CS_DMC_B02_latch_fifo_reset(ushort nCardNo, byte bEnable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_latch_fifo_status")]
        public static extern short CS_DMC_B02_latch_fifo_status(ushort nCardNo, ref byte nStatus);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_latch_fifo_size")]
        public static extern short CS_DMC_B02_latch_fifo_size(ushort nCardNo, ref byte nSize);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_latch_fifo_get_qep_counter")]
        public static extern short CS_DMC_B02_latch_fifo_get_qep_counter(ushort nCardNo, ref int nCounter, ref byte nIoType, ref byte nQepSource);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_fifo_size")]
        public static extern short CS_DMC_B02_tpcmp_fifo_size(ushort nCardNo, byte nFifoIndex, ref uint nSize);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_fifo_status")]
        public static extern short CS_DMC_B02_tpcmp_fifo_status(ushort nCardNo, byte nFifoIndex, ref byte nStatus);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_B02_tpcmp_compare_data_lock")]
        public static extern short CS_DMC_B02_tpcmp_compare_data_lock(ushort nCardNo, byte nFifoIndex, ref byte nLock);

        //20160121 (TableMotion) and (PVT) and (DDA Table2) and()
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_axis_normalamf_enable")]
        public static extern short CS_DMC_01_set_axis_normalamf_enable(ushort CardNo, ushort NodeID, ushort SlotID, ushort filterTime, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_normalamf_enable")]
        public static extern short CS_DMC_01_set_normalamf_enable(ushort CardNo, ushort afteramf_Num, ushort enable);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion")]
        public static extern short CS_DMC_01_start_tablemotion(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, int DataSize, short AxisNum, int MaxVel, double Tacc_Tdec, ushort AMFNum, ushort cycle, ref uint Motion_No, ref int IOCtl);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_buffer")]
        public static extern short CS_DMC_01_start_tablemotion_buffer(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, int DataSize, short AxisNum, int MaxVel, double Tacc_Tdec, ushort Single_Step_Enable, ref uint Motion_No, ushort AMFNum, ref int IOCtl);

        //2017-01-05
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion2")]
        public static extern short CS_DMC_01_start_tablemotion2(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, ref int MaxVelocity, int DataSize, short AxisNum, double Tacc_Tdec, ushort AMFNum, ushort cycle, ref uint Motion_No, ref int IOCtl);
        //2017-01-05
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_buffer2")]
        public static extern short CS_DMC_01_start_tablemotion_buffer2(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, ref int MaxVelocity, int DataSize, short AxisNum, double Tacc_Tdec, ushort Single_Step_Enable, ref uint Motion_No, ushort AMFNum, ref int IOCtl);

        //2017-06-01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion3")]
        public static extern short CS_DMC_01_start_tablemotion3(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, ref int Ref_Absolute_Pos, ref int MaxVelocity, int DataSize, short AxisNum, double Tacc_Tdec, ushort AMFNum, ushort cycle, ref uint ArcBit, ref uint Motion_No, ref double Delay_Cyc);

        //2017-06-05
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_buffer3")]
        public static extern short CS_DMC_01_start_tablemotion_buffer3(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int PositionData, ref int Ref_Absolute_Pos, ref int MaxVelocity, int DataSize, short AxisNum, double Tacc_Tdec, ushort Single_Step_Enable, ref uint ArcBit, ref uint Motion_No, ref uint Delay_CycleCount,ushort AMFNum, ushort Abs_Mode,ref uint [] IOCtl);


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_cyclemode")]
        public static extern short CS_DMC_01_start_tablemotion_cyclemode(ushort CardNo, short TableNo);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_tablemotion_length")]
        public static extern short CS_DMC_01_get_tablemotion_length(ushort CardNo, short TableNo, ref int cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_tablemotion_cycle")]
        public static extern short CS_DMC_01_get_tablemotion_cycle(ushort CardNo, short TableNo, ref int cycle);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_tablemotion_feedrate_overwrite")]
        public static extern short CS_DMC_01_tablemotion_feedrate_overwrite(ushort CardNo, short TableNo, int ratio, double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_tablemotion_feedhold")]
        public static extern short CS_DMC_01_tablemotion_feedhold(ushort CardNo, short TableNo, ushort On_Off, double sec);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_tablemotion_feedrate")]
        public static extern short CS_DMC_01_get_tablemotion_feedrate(ushort CardNo, short TableNo, ref short feedrate);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_tablemotion_vector_speed")]
        public static extern short CS_DMC_01_get_tablemotion_vector_speed(ushort CardNo, short TableNo, ref double vectorspeed);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_tablemotion_blending_ratio")]
        public static extern short CS_DMC_01_set_tablemotion_blending_ratio(ushort CardNo, short TableNo, uint Ratio);
        //20160607
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_tablemotion_current_speed")]
        public static extern short CS_DMC_01_get_tablemotion_current_speed(ushort CardNo, short TableNo, ref int CurrentSpeed);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dmc_cycle_int_count")]
        public static extern short CS_DMC_01_set_dmc_cycle_int_count(ushort CardNo, ushort count);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_profile_planning")]
        public static extern short CS_DMC_01_start_tablemotion_profile_planning(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int AccDec, ref int PositionData, int DataSize, short AxisNum, ref int MaxVel, double InitialVel_Ratio, ref int IOCtl, ref uint Motion_No);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_profile_planning_para")]
        public static extern short CS_DMC_01_start_tablemotion_profile_planning_para(ushort CardNo, short TableNo, int NanoSmoothDecectionLength, int RefTurnOnSpd, short CurveDecectionFlag, int CurveDecectionLength, double CurveDecectionAngle, int ArcRefSpeed, int ArcRefRadius);

        //2017-06-07
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_profile_planning2")]
        public static extern short CS_DMC_01_start_tablemotion_profile_planning2(ushort CardNo, ref ushort NodeID, ref ushort SlotID, short TableNo, ref int AccDec, ref int PositionData, ref int Ref_Absolute_Pos, int DataSize, short AxisNum, ref int MaxVel, double InitialVel_Ratio, ref int IOCtl, ref uint ArcBit, ref uint M);


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_tablemotion_profilemode")]
        public static extern short CS_DMC_01_start_tablemotion_profilemode(ushort CardNo, short TableNo);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_tablemotion_ioctl_mapping")]
        public static extern short CS_DMC_01_set_tablemotion_ioctl_mapping(ushort CardNo, short TableNo, ushort ionum, ushort NodeID, ushort SlotID, ushort iobit);
        //20160421
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Start_PVTComplete")]
        public static extern short CS_DMC_01_Start_PVTComplete(ushort CardNo, ushort NodeID, ushort SlotID, ref double Position, ref double Time, double Str_Vel, double End_Vel, int MotionCnt, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Start_PVT")]
        public static extern short CS_DMC_01_Start_PVT(ushort CardNo, ushort NodeID, ushort SlotID, ref double Position, ref double Time, ref double Velocity, int MotionCnt, ushort m_r_a);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_PVT_cnt")]
        public static extern short CS_DMC_01_get_PVT_cnt(ushort CardNo, ushort NodeID, ushort SlotID, ref uint count);

        //
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_enable_dda_mode_group")]
        public static extern short CS_DMC_01_enable_dda_mode_group(ushort CardNo, ushort GroupNo, ushort enable);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dda_data_group")]
        public static extern short CS_DMC_01_set_dda_data_group(ushort CardNo, ushort GroupNo, ref uint abs_pos);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dda_cnt_group")]
        public static extern short CS_DMC_01_get_dda_cnt_group(ushort CardNo, ushort GroupNo, ref ushort dda_cnt);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_clear_dda_data_group")]
        public static extern short CS_DMC_01_clear_dda_data_group(ushort CardNo, ushort GroupNo);

        //20160205 DMC模式扭矩回home
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_home_config_by_torque_limit")]
        public static extern short CS_DMC_01_set_home_config_by_torque_limit(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode, int offset, ushort lowSpeed, ushort highSpeed, double acc, ushort torque_ratio, ushort qualified_ms, ushort max_torque_ratio);

        //20160318
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_v3_heli_multi_axes_move")]
        public static extern short CS_DMC_01_start_v3_heli_multi_axes_move(ushort CardNo, ushort AxisNum, ref ushort NodeID, ref ushort SlotID, int Center_X, int Center_Y, ref int Dist, int Depth, int Pitch, short Dir, int StrVel, int ConstVel, int EndVel, double Tphase1, double Tphase2, ushort m_curve, ushort m_r_a);

        //20160401(single bit)
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_input_single_value")]
        public static extern short CS_DMC_01_get_rm_input_single_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort bitno, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_rm_output_single_value")]
        public static extern short CS_DMC_01_get_rm_output_single_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort bitno, ref ushort value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_rm_output_single_value")]
        public static extern short CS_DMC_01_set_rm_output_single_value(ushort CardNo, ushort NodeID, ushort SlotID, ushort port, ushort bitno, ushort value);

        //20160406
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_single_output")]
        public static extern short CS_DMC_01_set_dio_single_output(ushort CardNo, ushort bitno, ushort On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_output")]
        public static extern short CS_DMC_01_get_dio_single_output(ushort CardNo, ushort bitno, ref ushort On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_input")]
        public static extern short CS_DMC_01_get_dio_single_input(ushort CardNo, ushort bitno, ref ushort On_Off);


        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_single_output_DW")]
        public static extern short CS_DMC_01_set_dio_single_output_DW(ushort CardNo, ushort bitno, uint On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_output_DW")]
        public static extern short CS_DMC_01_get_dio_single_output_DW(ushort CardNo, ushort bitno, ref uint On_Off);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_input_DW")]
        public static extern short CS_DMC_01_get_dio_single_input_DW(ushort CardNo, ushort bitno, ref uint On_Off);

        //20160503 MP1 Security
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Check_Verifykey")]
        public static extern ushort CS_DMC_01_Security_Check_Verifykey(ushort CardNo, ref uint[] VerifyKey);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Get_Check_Verifykey_State")]
        public static extern ushort CS_DMC_01_Security_Get_Check_Verifykey_State(ushort CardNo, ref ushort State);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Write_Verifykey")]
        public static extern ushort CS_DMC_01_Security_Write_Verifykey(ushort CardNo, ref uint[] VerifyKey);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Get_Write_Verifykey_State")]
        public static extern ushort CS_DMC_01_Security_Get_Write_Verifykey_State(ushort CardNo, ref ushort State);

        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Check_UserPassword")]
        public static extern ushort CS_DMC_01_Security_Check_UserPassword(ushort CardNo, ref uint[] Password);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Get_Check_UserPassword_State")]
        public static extern ushort CS_DMC_01_Security_Get_Check_UserPassword_State(ushort CardNo, ref ushort State);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Write_UserPassword")]
        public static extern ushort CS_DMC_01_Security_Write_UserPassword(ushort CardNo, ref uint[] Password);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Security_Get_Write_UserPassword_State")]
        public static extern ushort CS_DMC_01_Security_Get_Write_UserPassword_State(ushort CardNo, ref ushort State);

        //20160825
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dda_error_global")]
        public static extern short CS_DMC_01_get_dda_error_global(ushort CardNo, ref uint error_global);

        //20160920
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Set_PID_Parameters")]
        public static extern short CS_DMC_01_Set_PID_Parameters(short CardNo, short PIDNo, double Proportional_Gain, double Integrat_Gain, double Dervative_Gain, double IntegratMax, double IntegratMin, double Command_Max, double Command_Min, double Feedback_Max, double Feedback_Min, int cycleTime);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_PID_Set_Source_Target_Info")]
        public static extern short CS_DMC_01_PID_Set_Source_Target_Info(ushort CardNo, ushort PIDNo, ushort OutputNodeNo, ushort OutputSlotNo, ushort OutputChannelNo, ushort FeedbackNodeNo, ushort FeedbackSlotNo, ushort FeedbackChannelNo, ushort Enable);

        //20160923
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_PID_Set_DA_Vaule")]
        public static extern short CS_DMC_01_PID_Set_DA_Vaule(ushort CardNo, ushort PIDNo, double DA_Value);

        //2016-11-15 for M-R
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_pdo_Wdata")]
        public static extern short CS_DMC_01_set_pdo_Wdata(ushort CardNo, ushort NodeID, ushort slot, ushort Value0, ushort Value1, ushort Value2, ushort Value3);

        //2016-11-15 for M-R
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_pdo_Wdata")]
        public static extern short CS_DMC_01_get_pdo_Wdata(ushort CardNo, ushort NodeID, ushort slot, ref ushort Value0, ref ushort Value1, ref ushort Value2, ref ushort Value3);

        //2016-11-15 for M-R Dword
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_pdo_DWdata")]
        public static extern short CS_DMC_01_set_pdo_DWdata(ushort CardNo, ushort NodeID, ushort slot, uint Value0, uint Value1);

        //2016-11-15 for M-R Dword
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_pdo_DWdata")]
        public static extern short CS_DMC_01_get_pdo_DWdata(ushort CardNo, ushort NodeID, ushort slot, ref uint Value0, ref uint Value1);

        //2016-11-22 
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_start_multiarc")]
        public static extern short CS_DMC_01_start_multiarc(short CardNo, ushort AxesNum, ref ushort NodeID, ref ushort SlotID, ushort ArcBit, ushort HelixBit, ushort ArcMode, ref int CenterPoint, ref int EndPoint, double Angle, int StrVel, int ConstVel, int EndVel, double Tacc, double Tdec, ushort Curve_mode, ushort AbsMo);
        //2016-12-06
        [DllImport("PCI_DMC.dll", EntryPoint = "_stdcall")]
        public static extern short CS_stdcall();

        //2016-12-06
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_command_mapping")]
        public static extern short CS_DMC_01_set_command_mapping(short CardNo, short NodeID, short SlotID);

        //2017-01-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Execute_Single_Step")]
        public static extern short CS_DMC_01_Execute_Single_Step(short CardNo, short TableNo);

        //2017-01-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Get_TableMotion_Now_MotionNo")]
        public static extern short CS_DMC_01_Get_TableMotion_Now_MotionNo(short CardNo, short TableNo, ref uint MotionNo);

        //2017-01-20
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_TableMotion_Single_Step")]
        public static extern short CS_DMC_01_TableMotion_Single_Step(short CardNo, short TableNo, ushort Enable);

        //2017-02-10
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_trigger_mode")]
        public static extern short CS_DMC_01_set_dio_input_trigger_mode(ushort CardNo, ushort trigger_mode);
        //2017-02-10
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_int_enable")]
        public static extern short CS_DMC_01_set_dio_input_int_enable(ushort CardNo, ushort Enable);
        //2017-02-24
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_input_int_counter")]
        public static extern short CS_DMC_01_get_dio_input_int_counter(ushort CardNo, ref ushort counter);
        //2017-02-24
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_int_counter_clear")]
        public static extern short CS_DMC_01_set_dio_input_int_counter_clear(ushort CardNo);
        //2017-03-01
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_input_int_status")]
        public static extern short CS_DMC_01_get_dio_input_int_status(ushort CardNo, ref ushort status);


        //2017-02-14
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_level_emg")]
        public static extern short CS_DMC_01_set_dio_input_level_emg(ushort CardNo, ushort Enable);
        //2017-02-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_trigger_level")]
        public static extern short CS_DMC_01_set_dio_input_trigger_level(ushort CardNo, ushort level_mode);
        //2017-02-17
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_input_level_emg_clear")]
        public static extern short CS_DMC_01_set_dio_input_level_emg_clear(ushort CardNo);
        //2017-02-20
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_input_level_emg_status")]
        public static extern short CS_DMC_01_get_dio_input_level_emg_status(ushort CardNo, ref ushort status);

        //2017-03-10
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_buffer_correlation_master_slave")]
        public static extern short CS_DMC_01_buffer_correlation_master_slave(short CardNo, ushort Slave_Node_Num, ushort Master_NodeID, ref ushort Slave_NodeID, ushort Slave_SlotID, short Enable);
        //2017-03-13
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_buffer_correlation_status")]
        public static extern short CS_DMC_01_buffer_correlation_status(short CardNo, ref ushort Master_NodeID, ref ushort correlation_NodeID, ref ushort correlation_enable);

        // 2017-03-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_pdo_data16")]
        public static extern short CS_DMC_01_get_pdo_data16(ushort CardNo, ushort NodeID, ushort SlotID, ushort Index, ref ushort Value);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_pdo_data32")]
        public static extern short CS_DMC_01_get_pdo_data32(ushort CardNo, ushort NodeID, ushort SlotID, ushort Index, ref uint Value);

        // 2017-03-30
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_reference_pos_move_config")]
        public static extern short CS_DMC_01_reference_pos_move_config(ushort CardNo, ushort MasterNodeID, ushort MasterSlotID, ushort SlaveNodeID, ushort SlaveSlotID, int ReferencePos, short Dir);
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_sd_stop_and_feedhold")]
        public static extern short CS_DMC_01_sd_stop_and_feedhold(ushort CardNo, ushort NodeID, ushort SlotID, double Tdec, ushort Mode);

        // 2017-04-05
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_svon_mode_of_operation")]
        public static extern short CS_DMC_01_set_svon_mode_of_operation(ushort CardNo, ushort NodeID, ushort SlotID, ushort Mode);

        //2017-04-06 //RD 內部使用 量DSP時間
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dsp_time")]
        public static extern short CS_DMC_01_get_dsp_time(ushort CardNo, ref int t1_buf_work, ref int t2_buf_work, ref int t1_interpolation, ref int t2_interpolation, ref int t1_cal_pos, ref int t2_cal_pos);

        //2017-04-06 //RD 內部使用 量DSP時間
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dsp_time")]
        public static extern short CS_DMC_01_set_dsp_time(ushort CardNo);


        //2017-06-13
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_single_output_DW_trigger_time")]
        public static extern short CS_DMC_01_set_dio_single_output_DW_trigger_time(ushort CardNo, uint bitno, double time_s, ushort Enable);

        //2017-06-13
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_single_output_DW_one_shot")]
        public static extern short CS_DMC_01_set_dio_single_output_DW_one_shot(ushort CardNo);

        //2017-06-13
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_output_DW_trigger_status")]
        public static extern short CS_DMC_01_get_dio_single_output_DW_trigger_status(ushort CardNo, ref ushort bitno, ref double time_s, ref ushort trigger_flag, ref ushort Enable);


        //2017-06-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_set_dio_single_input_DW_trigger_mode")]
        public static extern short CS_DMC_01_set_dio_single_input_DW_trigger_mode(ushort CardNo, ushort DI_bitno, ushort DO_bitno, ushort DO_polarity, ushort trigger_mode, ushort Enable);

        //2017-06-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_input_DW_trigger_mode")]
        public static extern short CS_DMC_01_get_dio_single_input_DW_trigger_mode(ushort CardNo, ref ushort DI_bitno, ref ushort DO_bitno, ref ushort DO_polarity, ref ushort trigger_mode, ref ushort Enable);

        //2017-06-16
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_dio_single_input_DW_trigger_flag")]
        public static extern short CS_DMC_01_get_dio_single_input_DW_trigger_flag(ushort CardNo, ref ushort DI_trigger_flag);

        //2017-10-31
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_get_fpga_version")]
        public static extern short CS_DMC_01_get_fpga_version(ushort CardNo, ref ushort ver);

        //2017-11-24
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Start_Move_Spiral_Circle")]
        public static extern short CS_DMC_01_Start_Move_Spiral_Circle(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int center_x, int center_y, int spiral_interval, uint spiral_angle, int StrVel, int MaxVel, int EndVel, double Tacc, double Tdec, short Curve_mode, short m_r_a);

        //2017-11-24
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_Start_Move_Spiral2_Circle")]
        public static extern short CS_DMC_01_Start_Move_Spiral2_Circle(ushort CardNo, ref ushort NodeID, ref ushort SlotID, int center_x, int center_y, int end_x, int end_y, ushort dir, ushort circlenum, int StrVel, int MaxVel, int EndVel, double Tacc, double Tdec, short Curve_mode, short m_r_a);

        //2017-12-08
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_ROUNDING")]
        public static extern short CS_DMC_01_ROUNDING(short CardNo, ushort Enable);

        //2017-12-15
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_feedrate_overwrite_read")]
        public static extern short CS_DMC_01_feedrate_overwrite_read(ushort CardNo, ushort NodeID, ushort SlotID, ref ushort Mode, ref int New_Speed, ref double sec);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Set_Crd_Parameters")]
        public static extern short CS_DMC_01_MotionBuf_Set_Crd_Parameters(short CardNo, short TableNo, short AxisNum, ref short AxisArray, int SycMaxVel, int SycMaxAcc, ushort S_Curve_Time, ushort Max_IO_Advance_Time, short SetOriginFlag, ref int OriginPos);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Crd_Parameters")]
        public static extern short CS_DMC_01_MotionBuf_Get_Crd_Parameters(short CardNo, short TableNo, ref short AxisNum, ref short AxisArray, ref int SycMaxVel, ref int SycMaxAcc, ref ushort S_Curve_Time, ref ushort Max_IO_Advance_Time, ref short SetOriginFlag, ref int OriginPos);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Line_MultiAxis")]
        public static extern short CS_DMC_01_MotionBuf_Line_MultiAxis(short CardNo, short TableNo, ref int TargetPosition, int SynTargetVel, int SynEndVel, int SynAcc, ushort AbsMode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Line_MultiAxis_G00")]
        public static extern short CS_DMC_01_MotionBuf_Line_MultiAxis_G00(short CardNo, short TableNo, ref int TargetPosition, int SynTargetVel, int SynAcc, ushort AbsMode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Arc_Radius")]
        public static extern short CS_DMC_01_MotionBuf_Arc_Radius(short CardNo, short TableNo, ref short Arc_Axis, ref int Arc_TargetPosition, int Radius, short CircleDir, int SynTargetVel, int SynEndVel, int SynAcc, ushort AbsMode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Arc_CenterPos")]
        public static extern short CS_DMC_01_MotionBuf_Arc_CenterPos(short CardNo, short TableNo, ref short Arc_Axis, ref int TargetPosition, ref int CenterPos, short CircleDir, int SynTargetVel, int SynEndVel, int SynAcc, ushort AbsMode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Set_LookAhead_Parameters")]
        public static extern short CS_DMC_01_MotionBuf_Set_LookAhead_Parameters(short CardNo, short TableNo, int RefSwerveSpd, short ArcSpdModify, int ArcRefSpeed, int ArcRefRadius, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Start")]
        public static extern short CS_DMC_01_MotionBuf_Start(short CardNo, short TableNo, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Stop")]
        public static extern short CS_DMC_01_MotionBuf_Stop(short CardNo, short TableNo, short FifoNo, short Mode);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Position")]
        public static extern short CS_DMC_01_MotionBuf_Get_Position(short CardNo, short TableNo, ref int Now_Position, ref int Now_Position_org);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Velocity")]
        public static extern short CS_DMC_01_MotionBuf_Get_Velocity(short CardNo, short TableNo, ref int CurrentSpeed);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Feedrate_Override")]
        public static extern short CS_DMC_01_MotionBuf_Feedrate_Override(short CardNo, short TableNo, double SynVelRatio);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Motion_Status")]
        public static extern short CS_DMC_01_MotionBuf_Get_Motion_Status(short CardNo, short TableNo, ref short Status, ref int MD_BlockNum, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Set_Stop_Deceleration")]
        public static extern short CS_DMC_01_MotionBuf_Set_Stop_Deceleration(short CardNo, short TableNo, int DecSmoothStop, int DecAbruptStop);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Stop_Deceleration")]
        public static extern short CS_DMC_01_MotionBuf_Get_Stop_Deceleration(short CardNo, short TableNo, ref int DecSmoothStop, ref int DecAbruptStop);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_SetUserSegNum")]
        public static extern short CS_DMC_01_MotionBuf_SetUserSegNum(short CardNo, short TableNo, int SegNum, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_GetUserSegNum")]
        public static extern short CS_DMC_01_MotionBuf_GetUserSegNum(short CardNo, short TableNo, ref int SegNum, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Clear_Buffer")]
        public static extern short CS_DMC_01_MotionBuf_Clear_Buffer(short CardNo, short TableNo, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Single_Step")]
        public static extern short CS_DMC_01_MotionBuf_Single_Step(short CardNo, short TableNo, short FifoNo, ushort Enable);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_BufGear")]
        public static extern short CS_DMC_01_MotionBuf_BufGear(short CardNo, short TableNo, short GearAxisNo, int Distance, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_BufMove")]
        public static extern short CS_DMC_01_MotionBuf_BufMove(short CardNo, short TableNo, short AxisNo, int Distance, int TargetVel, int Acceleration, ushort AbsMode, ushort BufMove_Mode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_IOControl")]
        public static extern short CS_DMC_01_MotionBuf_IOControl(short CardNo, short TableNo, short IO_Control_Num, ref IO_Set IO_Array, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Advanced_IOControl")]
        public static extern short CS_DMC_01_MotionBuf_Advanced_IOControl(short CardNo, short TableNo, short IO_Control_Num, ref IO_Set IO_Array, ushort Time, ushort Mode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Delay")]
        public static extern short CS_DMC_01_MotionBuf_Delay(short CardNo, short TableNo, double DelayTime, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_Get_Remain_Buffer_Size")]
        public static extern short CS_DMC_01_MotionBuf_Get_Remain_Buffer_Size(short CardNo, short TableNo, ref int RemainBufSize, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_SoftLimit_On")]
        public static extern short CS_DMC_01_MotionBuf_SoftLimit_On(short CardNo, short TableNo, short CD_AxisNo, short LimitType, ushort Mode, short FifoNo);

        //2017-12-27
        [DllImport("PCI_DMC.dll", EntryPoint = "_DMC_01_MotionBuf_SoftLimit_Off")]
        public static extern short CS_DMC_01_MotionBuf_SoftLimit_Off(short CardNo, short TableNo, short CD_AxisNo, short LimitType, short FifoNo);














    }
}
