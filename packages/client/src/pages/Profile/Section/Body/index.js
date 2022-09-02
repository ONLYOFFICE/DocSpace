import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

import MainProfile from "./sub-components/main-profile";
import LoginSettings from "./sub-components/login-settings";
import Subscription from "./sub-components/subscription";
import InterfaceTheme from "./sub-components/interface-theme";

import { tablet } from "@docspace/components/utils/device";

const Wrapper = styled.div`
  max-width: 660px;
  display: flex;
  flex-direction: column;
  gap: 40px;

  @media ${tablet} {
    width: 100%;
  }
`;

const SectionBodyContent = (props) => {
  const {
    t,
    profile,
    culture,
    resetTfaApp,
    getNewBackupCodes,
    backupCodes,
    setBackupCodes,
    getTfaType,
    getBackupCodes,
    changeEmailSubscription,
    tipsSubscription,
    updateProfile,
  } = props;
  const [tfa, setTfa] = useState(false);
  const [backupCodesCount, setBackupCodesCount] = useState(0);

  const fetchData = async () => {
    const type = await getTfaType();
    setTfa(type);
    if (type && type !== "none") {
      const codes = await getBackupCodes();
      setBackupCodes(codes);

      let backupCodesCount = 0;
      if (codes && codes.length > 0) {
        codes.map((item) => {
          if (!item.isUsed) {
            backupCodesCount++;
          }
        });
      }
      setBackupCodesCount(backupCodesCount);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <Wrapper>
      <MainProfile t={t} profile={profile} updateProfile={updateProfile} />
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
      <Subscription
        t={t}
        changeEmailSubscription={changeEmailSubscription}
        tipsSubscription={tipsSubscription}
      />
      <InterfaceTheme />
    </Wrapper>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { tfaStore } = auth;

    const { targetUserStore } = peopleStore;
    const {
      targetUser: profile,
      changeEmailSubscription,
      tipsSubscription,
      updateProfile,
    } = targetUserStore;

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
      getBackupCodes,
      getNewBackupCodes,
      resetTfaApp,
      getTfaType,
      backupCodes,
      setBackupCodes,
      changeEmailSubscription,
      tipsSubscription,
      updateProfile,
    };
  })(
    observer(
      withTranslation([
        "Profile",
        "Common",
        "PeopleTranslations",
        "ProfileAction",
      ])(withPeopleLoader(SectionBodyContent)(<Loaders.ProfileView />))
    )
  )
);
