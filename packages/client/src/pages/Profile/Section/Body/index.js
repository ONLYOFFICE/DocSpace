import React, { useEffect, useState } from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { useNavigate } from "react-router-dom";

import Loaders from "@docspace/common/components/Loaders";
import Submenu from "@docspace/components/submenu";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import MainProfile from "./sub-components/main-profile";
import LoginContent from "./sub-components/LoginContent";
import Notifications from "./sub-components/notifications";
import InterfaceTheme from "./sub-components/interface-theme";

import { tablet, hugeMobile } from "@docspace/components/utils/device";

const Wrapper = styled.div`
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
  const { isProfileLoaded, profile, t } = props;
  const navigate = useNavigate();

  console.log("profile", profile);

  const data = [
    {
      id: "login",
      name: t("Login"),
      content: <LoginContent />,
    },
    {
      id: "notifications",
      name: t("Notifications:Notifications"),
      content: <Notifications />,
    },
    {
      id: "interface-theme",
      name: t("InterfaceTheme"),
      content: <InterfaceTheme />,
    },
  ];

  if (!profile?.isVisitor)
    data.splice(2, 0, {
      id: "file-management",
      name: t("FileManagement"),
      content: <h1>File management</h1>,
    });

  const getCurrentTab = () => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    return currentTab !== -1 ? currentTab : 0;
  };

  const currentTab = getCurrentTab();

  const onSelect = (e) => {
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/profile/${e.id}`
      )
    );
  };

  if (!isProfileLoaded) return <Loaders.ProfileView />;
  return (
    <Wrapper>
      <MainProfile />
      <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />
    </Wrapper>
  );
};

export default inject(({ peopleStore, clientLoadingStore }) => {
  const { isProfileLoaded } = clientLoadingStore;
  const { targetUser: profile } = peopleStore.targetUserStore;

  return {
    isProfileLoaded,
    profile,
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
