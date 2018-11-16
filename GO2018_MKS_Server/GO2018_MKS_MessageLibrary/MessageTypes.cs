namespace GO2018_MKS_MessageLibrary
{
    public enum MessageType
    {
        generic,

        login,
        loginAnswer,

        logout,
        welcome,

        createSession,
        createSessionAnswer,
        abortCreateSession,

        waitForOpponent,
        waitForOpponentAnswer,
        abortWaitForOpponent,

        listSessions,
        listSessionsAnswer,
        joinSession,
        joinSessionAnswer,
        startCreatedSessionAnswer,

        readySessionStart,
        readySessionStartAnswer,
    }
}