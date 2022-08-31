import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

import MainProfile from "./sub-components/main-profile";
import LoginSettings from "./sub-components/login-settings";

const Wrapper = styled.div`
  max-width: 660px;
  display: flex;
  flex-direction: column;
  gap: 40px;
`;

const SectionBodyContent = (props) => {
  const {
    t,
    profile,
    culture,
    resetTfaApp,
    getNewBackupCodes,
    backupCodes,
    backupCodesCount,
    setBackupCodes,
    getTfaType,
    getBackupCodes,
  } = props;
  const [tfa, setTfa] = useState(false);

  const fetchData = async () => {
    const type = await getTfaType();
    setTfa(type);
    if (type && type !== "none") {
      const codes = await getBackupCodes();
      setBackupCodes(codes);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <Wrapper>
      <MainProfile t={t} profile={profile} culture={culture} />
      {tfa && tfa !== "none" && (
        <LoginSettings
          t={t}
          profile={profile}
          resetTfaApp={resetTfaApp}
          getNewBackupCodes={getNewBackupCodes}
          backupCodes={backupCodes}
          backupCodesCount={backupCodesCount}
          setBackupCodes={setBackupCodes}
        />
      )}
    </Wrapper>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { settingsStore, tfaStore } = auth;
    const { culture } = settingsStore;

    const { targetUserStore } = peopleStore;
    const { targetUser: profile } = targetUserStore;

    const {
      getBackupCodes,
      getNewBackupCodes,
      unlinkApp: resetTfaApp,
      getTfaType,
      backupCodes,
      setBackupCodes,
    } = tfaStore;

    return {
      profile,
      culture,
      getBackupCodes,
      getNewBackupCodes,
      resetTfaApp,
      getTfaType,
      backupCodes,
      setBackupCodes,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withPeopleLoader(SectionBodyContent)(<Loaders.ProfileView />)
      )
    )
  )
);
