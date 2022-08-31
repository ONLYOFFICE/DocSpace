import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

import MainProfile from "./sub-components/main-profile";

const Wrapper = styled.div`
  max-width: 660px;
`;

const SectionBodyContent = (props) => {
  const { t, profile, culture } = props;
  return (
    <Wrapper>
      <MainProfile t={t} profile={profile} culture={culture} />
    </Wrapper>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { settingsStore } = auth;
    const { culture } = settingsStore;

    const { targetUserStore } = peopleStore;
    const { targetUser: profile } = targetUserStore;

    return {
      profile,
      culture,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withPeopleLoader(SectionBodyContent)(<Loaders.ProfileView />)
      )
    )
  )
);
