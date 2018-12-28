namespace GO2018_MKS_MessageLibrary
{
    public enum MessageType
    {
        generic, // 0

        login, // 1
        loginAnswer, // 2

        logout, //3
        welcome, // 4

        createSession, // 5
        createSessionAnswer, // 6
        abortCreateSession, // 7

        waitForOpponent, // 8
        waitForOpponentAnswer, // 9
        abortWaitForOpponent, // 10

        listSessions, // 11
        listSessionsAnswer, // 12
        joinSession, // 13
        joinSessionAnswer, // 14
        startCreatedSessionAnswer, // 15

        readySessionStart, // 16
        readySessionStartAnswer, // 17

        playerSessionLost, // 18
        opponentSessionLostAnswer, // 19

        endSessionAnswer, // 20

        sessionUpdateAnswer, // 21 
        playerUnitsNavigation, // 22
        playerUnitsUpdate, // 23
        mineResourcesUpdate, // 24
        barricadesUpdate, // 25

        sessionChat, // 26
        sessionChatAnswer // 27
    }

    public static class MessageTypeTexts
    {
        public static string[] Text =
        {
            "generic", // 0

            "login", // 1
            "loginAnswer", // 2

            "logout", // 3
            "welcome", // 4

            "reateSession", // 5
            "createSessionAnswer", // 6
            "abortCreateSession", // 7

            "waitForOpponent", // 8
            "waitForOpponentAnswer", // 9
            "abortWaitForOpponent", // 10

            "listSessions", // 11
            "listSessionsAnswer", // 12
            "joinSession", // 13
            "joinSessionAnswer", // 14
            "startCreatedSessionAnswer", // 15

            "readySessionStart", // 16
            "readySessionStartAnswer", // 17

            "playerSessionLost", // 18
            "opponentSessionLostAnswer", // 19

            "endSessionAnswer", // 20

            "sessionUpdateAnswer", // 21
            "playerUnitsNavigation", // 22
            "playerUnitsUpdate", // 23
            "mineResourcesUpdate", // 24
            "barricadesUpdate" // 25
        };

        public static string GetMessageTypeText(MessageType type)
        {
            return Text[(int)type];
        }
    }
}