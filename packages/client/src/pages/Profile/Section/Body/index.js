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
  const { setBackupCodes, getTfaType, getBackupCodes } = props;
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
      <MainProfile />
      {tfa && tfa !== "none" && (
        <LoginSettings backupCodesCount={backupCodesCount} />
      )}
      {/*<SocialNetworks /> TODO: uncomment after oAuth fix*/}
      <Subscription />
      <InterfaceTheme />
    </Wrapper>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { tfaStore } = auth;
    const { getBackupCodes, getTfaType, setBackupCodes } = tfaStore;

    return {
      getBackupCodes,
      getTfaType,
      setBackupCodes,
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
