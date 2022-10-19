import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import HelpButton from "@docspace/components/help-button";

import {
  StyledFooter,
  Table,
  TableHead,
  TableRow,
  TableHeaderCell,
  TableBody,
  TableDataCell,
} from "./styled-active-sessions";

const removeIcon = (
  <ReactSVG className="remove-icon" src="images/remove.react.svg" />
);
const tickIcon = (
  <ReactSVG className="tick-icon" wrapper="span" src="images/tick.svg" />
);

const ActiveSessions = ({
  userId,
  getAllSessions,
  removeAllSessions,
  removeSession,
}) => {
  const [sessions, setSessions] = useState([]);
  const [currentSession, setCurrentSession] = useState(0);
  const { t } = useTranslation(["Profile", "Common"]);

  useEffect(() => {
    getAllSessions().then((res) => {
      setSessions(res.items);
      setCurrentSession(res.loginEvent);
    });
  }, []);

  const onClickRemoveAllSessions = () => {
    if (sessions.length > 1) {
      removeAllSessions(userId).then(() =>
        getAllSessions().then((res) => setSessions(res.items))
      );
    }
  };

  const onClickRemoveSession = (id) => {
    const foundSession = sessions.find((s) => s.id === id);
    if (foundSession.id !== currentSession) {
      removeSession(foundSession.id).then(() =>
        getAllSessions().then((res) => setSessions(res.items))
      );
    }
  };

  const convertTime = (date) => {
    return new Date(date).toLocaleString("en-US");
  };

  return (
    <StyledFooter>
      <Text fontSize="16px" fontWeight={700}>
        {t("Profile:ActiveSessions")}
      </Text>
      <Box
        displayProp="flex"
        alignItems="center"
        justifyContent="flex-start"
        marginProp="10px 0 0"
      >
        <Link
          className="session-logout"
          type="action"
          isHovered
          onClick={onClickRemoveAllSessions}
        >
          {t("Profile:LogoutAllActiveSessions")}
        </Link>
        <HelpButton
          iconName="/static/images/info.react.svg"
          tooltipContent={
            <Text fontSize="13px">
              {t("Profile:LogoutAllActiveSessionsDescription")}
            </Text>
          }
        />
      </Box>

      <Table>
        <TableHead>
          <TableRow>
            <TableHeaderCell>{t("Common:Sessions")}</TableHeaderCell>
            <TableHeaderCell>{t("Common:Date")}</TableHeaderCell>
            <TableHeaderCell>{t("Common:IpAddress")}</TableHeaderCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {sessions.map((session) => (
            <TableRow key={session.id}>
              <TableDataCell>
                {session.platform}
                <span>({session.browser})</span>
                {currentSession === session.id ? tickIcon : null}
              </TableDataCell>
              <TableDataCell>{convertTime(session.date)}</TableDataCell>
              <TableDataCell>{session.ip}</TableDataCell>
              <TableDataCell onClick={() => onClickRemoveSession(session.id)}>
                {currentSession !== session.id ? removeIcon : null}
              </TableDataCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </StyledFooter>
  );
};

export default inject(({ auth, setup }) => {
  const { getAllSessions, removeAllSessions, removeSession } = setup;
  return {
    userId: auth.userStore.user.id,
    logout: auth.logout,
    getAllSessions,
    removeAllSessions,
    removeSession,
  };
})(observer(ActiveSessions));
