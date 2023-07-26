import React, { useEffect, useState } from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Loaders from "@docspace/common/components/Loaders";

import MainProfile from "./sub-components/main-profile";
import LoginSettings from "./sub-components/login-settings";
import Subscription from "./sub-components/subscription";
import InterfaceTheme from "./sub-components/interface-theme";
import SocialNetworks from "./sub-components/social-networks";

import { tablet, hugeMobile } from "@docspace/components/utils/device";

const Wrapper = styled.div`
  max-width: 660px;
  display: flex;
  flex-direction: column;
  gap: 40px;

  @media ${tablet} {
    width: 100%;
    max-width: 100%;
  }

  @media ${hugeMobile} {
    gap: 32px;
  }
`;

const SectionBodyContent = (props) => {
  const {
    setBackupCodes,
    getTfaType,
    getBackupCodes,
    isProfileLoaded,

    t,
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

  if (!isProfileLoaded) return <Loaders.ProfileView />;

  return (
    <Wrapper>
      <MainProfile />
      {tfa && tfa !== "none" && (
        <LoginSettings backupCodesCount={backupCodesCount} />
      )}
      <SocialNetworks />
      <Subscription t={t} />
      <InterfaceTheme />
    </Wrapper>
  );
};

export default inject(({ auth, clientLoadingStore }) => {
  const { tfaStore } = auth;
  const { getBackupCodes, getTfaType, setBackupCodes } = tfaStore;
  const { isProfileLoaded } = clientLoadingStore;
  return {
    getBackupCodes,
    getTfaType,
    setBackupCodes,
    isProfileLoaded,
  };
})(
  observer(
    withTranslation([
      "Profile",
      "Common",
      "PeopleTranslations",
      "ProfileAction",
      "ResetApplicationDialog",
      "BackupCodesDialog",
      "DeleteSelfProfileDialog",
      "Notifications",
    ])(SectionBodyContent)
  )
);
