/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_ATTACKIMPACT = 1847283560U;
        static const AkUniqueID PLAY_ATTACKSWING = 3468848948U;
        static const AkUniqueID PLAY_DASH = 2211787386U;
        static const AkUniqueID PLAY_FIREBALL = 146533081U;
        static const AkUniqueID PLAY_FOOTSTEP = 1602358412U;
        static const AkUniqueID PLAY_HEAL = 2639148008U;
        static const AkUniqueID PLAY_HIT = 2960666077U;
        static const AkUniqueID PLAY_JUMP = 3689126666U;
        static const AkUniqueID PLAY_LAND = 4285282925U;
        static const AkUniqueID PLAY_WALLBREAK = 3767549391U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAME_STATE
        {
            static const AkUniqueID GROUP = 766723505U;

            namespace STATE
            {
                static const AkUniqueID BOSS_FIGHT = 3688152761U;
                static const AkUniqueID DEFEATED = 2791675679U;
                static const AkUniqueID GAMEPLAY = 89505537U;
                static const AkUniqueID MAIN_MENU = 2005704188U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSED = 319258907U;
            } // namespace STATE
        } // namespace GAME_STATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace ATTACKSWING_NUMBER
        {
            static const AkUniqueID GROUP = 788347211U;

            namespace SWITCH
            {
                static const AkUniqueID ATTACKSWING_1 = 1950540977U;
                static const AkUniqueID ATTACKSWING_2 = 1950540978U;
                static const AkUniqueID ATTACKSWING_3 = 1950540979U;
            } // namespace SWITCH
        } // namespace ATTACKSWING_NUMBER

        namespace ELEMENTAL_FORM
        {
            static const AkUniqueID GROUP = 1725842569U;

            namespace SWITCH
            {
                static const AkUniqueID AIR_FORM = 2482295332U;
                static const AkUniqueID FIRE_FORM = 699461374U;
                static const AkUniqueID NULL_FORM = 1066927599U;
            } // namespace SWITCH
        } // namespace ELEMENTAL_FORM

    } // namespace SWITCHES

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID LEVEL_FACTORY = 2858673722U;
        static const AkUniqueID LEVEL_FOREST = 957893059U;
        static const AkUniqueID PLAYER_CORE = 2509385384U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ENEMY_BUS = 1411040840U;
        static const AkUniqueID ENVIRONMENTAL_BUS = 3942603440U;
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MUSIC_BUS = 2680856269U;
        static const AkUniqueID NON_WORLD_BUS = 2239884906U;
        static const AkUniqueID PLAYER_BUS = 1138681361U;
        static const AkUniqueID UI_BUS = 3247222208U;
        static const AkUniqueID WORLD_BUS = 1836527144U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID AUX_FACTORY_LARGE = 825633738U;
        static const AkUniqueID AUX_FACTORY_MEDIUM = 3115457652U;
        static const AkUniqueID AUX_FACTORY_SMALL = 2498296054U;
        static const AkUniqueID AUX_FOREST_LARGE = 1015162927U;
        static const AkUniqueID AUX_FOREST_MEDIUM = 2096483835U;
        static const AkUniqueID AUX_FOREST_SMALL = 2958102843U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
