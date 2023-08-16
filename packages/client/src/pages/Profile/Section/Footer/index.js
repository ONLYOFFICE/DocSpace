import RemoveSessionSvgUrl from "PUBLIC_DIR/images/remove.session.svg?url";
import TickSvgUrl from "PUBLIC_DIR/images/tick.svg?url";
import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";
import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import HelpButton from "@docspace/components/help-button";
import toastr from "@docspace/components/toast/toastr";
import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

import {
  LogoutConnectionDialog,
  LogoutAllConnectionDialog,
} from "SRC_DIR/components/dialogs";

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
  <ReactSVG className="remove-icon" src={RemoveSessionSvgUrl} />
);
const tickIcon = (
  <ReactSVG className="tick-icon" wrapper="span" src={TickSvgUrl} />
);

const ActiveSessions = ({
  t,
  locale,
  getAllSessions,
  removeAllSessions,
  removeSession,
  logoutVisible,
  setLogoutVisible,
  logoutAllVisible,
  setLogoutAllVisible,
  removeAllExecptThis,
}) => {
  const [sessions, setSessions] = useState([]);
  const [currentSession, setCurrentSession] = useState(0);
  const [modalData, setModalData] = useState({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getAllSessions().then((res) => {
      setSessions(res.items);
      setCurrentSession(res.loginEvent);
    });
  }, []);

  const onClickRemoveAllSessions = async () => {
    try {
      setLoading(true);
      await removeAllSessions().then((res) => window.location.replace(res));
    } catch (error) {
      toastr.error(error);
    } finally {
      setLoading(false);
      setLogoutAllVisible(false);
    }
  };

  const onClickRemoveAllExceptThis = async () => {
    try {
      setLoading(true);
      await removeAllExecptThis().then(() =>
        getAllSessions().then((res) => setSessions(res.items))
      );
    } catch (error) {
      toastr.error(error);
    } finally {
      setLoading(false);
      setLogoutAllVisible(false);
    }
  };

  const onClickRemoveSession = async (id) => {
    const foundSession = sessions.find((s) => s.id === id);
    try {
      setLoading(true);
      await removeSession(foundSession.id).then(() =>
        getAllSessions().then((res) => setSessions(res.items))
      );
      toastr.success(
        t("Profile:SuccessLogout", {
          platform: foundSession.platform,
          browser: foundSession.browser,
        })
      );
    } catch (error) {
      toastr.error(error);
    } finally {
      setLoading(false);
      setLogoutVisible(false);
    }
  };

  const convertTime = (date) => {
    return new Date(date).toLocaleString(locale);
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
          onClick={() => setLogoutAllVisible(true)}
        >
          {t("Profile:LogoutAllActiveSessions")}
        </Link>
        <HelpButton
          offsetRight={0}
          iconName={InfoReactSvgUrl}
          tooltipContent={
            <Text fontSize="12px">
              {t("Profile:LogoutAllActiveSessionsDescription")}
            </Text>
          }
        />
      </Box>
      {isMobile ? (
        <Table>
          <TableBody>
            {sessions.map((session) => (
              <TableRow key={session.id}>
                <TableDataCell style={{ borderTop: "0" }}>
                  {session.platform}
                  <span className="session-browser">
                    <span>{session.browser}</span>
                  </span>
                  {currentSession === session.id ? tickIcon : null}

                  <Box flexDirection="column" alignItems="center">
                    <span className="session-date">
                      {convertTime(session.date)}
                    </span>
                    <span className="session-ip">{session.ip}</span>
                  </Box>
                </TableDataCell>

                <TableDataCell
                  style={{ borderTop: "0" }}
                  onClick={() => {
                    setLogoutVisible(true);
                    setModalData({
                      id: session.id,
                      platform: session.platform,
                      browser: session.browser,
                    });
                  }}
                >
                  {currentSession !== session.id ? removeIcon : null}
                </TableDataCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      ) : (
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
                  <span className="session-browser">
                    <span>{session.browser}</span>
                  </span>
                  {currentSession === session.id ? tickIcon : null}
                </TableDataCell>
                <TableDataCell>{convertTime(session.date)}</TableDataCell>
                <TableDataCell>{session.ip}</TableDataCell>
                <TableDataCell
                  onClick={() => {
                    setLogoutVisible(true);
                    setModalData({
                      id: session.id,
                      platform: session.platform,
                      browser: session.browser,
                    });
                  }}
                >
                  {currentSession !== session.id ? removeIcon : null}
                </TableDataCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}

      {logoutVisible && (
        <LogoutConnectionDialog
          visible={logoutVisible}
          data={modalData}
          loading={loading}
          onClose={() => setLogoutVisible(false)}
          onRemoveSession={onClickRemoveSession}
        />
      )}

      {logoutAllVisible && (
        <LogoutAllConnectionDialog
          visible={logoutAllVisible}
          loading={loading}
          onClose={() => setLogoutAllVisible(false)}
          onRemoveAllSessions={onClickRemoveAllSessions}
          onRemoveAllExceptThis={onClickRemoveAllExceptThis}
        />
      )}
    </StyledFooter>
  );
};

export default inject(({ auth, setup }) => {
  const { culture } = auth.settingsStore;
  const { user } = auth.userStore;
  const locale = (user && user.cultureName) || culture || "en";

  const {
    getAllSessions,
    removeAllSessions,
    removeSession,
    logoutVisible,
    setLogoutVisible,
    logoutAllVisible,
    setLogoutAllVisible,
    removeAllExecptThis,
  } = setup;
  return {
    locale,
    getAllSessions,
    removeAllSessions,
    removeSession,
    logoutVisible,
    setLogoutVisible,
    logoutAllVisible,
    setLogoutAllVisible,
    removeAllExecptThis,
  };
})(
  observer(
    withTranslation(["Profile", "Common"])(
      withPeopleLoader(ActiveSessions)(
        <Loaders.ProfileFooter isProfileFooter />
      )
    )
  )
);
