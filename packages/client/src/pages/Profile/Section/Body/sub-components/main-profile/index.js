import React from "react";
import styled from "styled-components";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import withCultureNames from "@docspace/common/hoc/withCultureNames";

import { getUserRole } from "../../../../../../helpers/people-helpers";

import LanguagesCombo from "./languagesCombo";

const StyledWrapper = styled.div`
  display: flex;
  padding: 24px;
  gap: 40px;
  background: #f8f9f9;
  border-radius: 12px;

  .avatar {
    height: 124px;
    width: 124px;
  }
`;

const StyledInfo = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  .row {
    display: flex;
    gap: 24px;

    .label {
      min-width: 60px;
      max-width: 60px;
      white-space: nowrap;
    }

    .field {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .lang-combo {
      & > div {
        padding: 0 !important;
      }
    }
  }
`;

const MainProfile = (props) => {
  const { t, profile, cultureNames, culture } = props;
  const { cultureName, currentCulture } = profile;

  console.log(profile);
  const role = getUserRole(profile);

  return (
    <StyledWrapper>
      <Avatar
        className="avatar"
        size="max"
        role={role}
        source={profile.avatarMax}
        userName={profile.displayName}
        editing={true}
        editAction={() => console.log("edit")}
      />
      <StyledInfo>
        <div className="row">
          <Text as="div" color="#A3A9AE" className="label">
            {t("Common:Name")}
          </Text>
          <div className="field">
            <Text fontWeight={600}>{profile.displayName}</Text>
            <IconButton
              iconName="/static/images/pencil.outline.react.svg"
              size="12"
              onClick={() => console.log("onclick")}
            />
          </div>
        </div>
        <div className="row">
          <Text as="div" color="#A3A9AE" className="label">
            {t("Common:Email")}
          </Text>
          <div className="field">
            <Text fontWeight={600}>{profile.email}</Text>
            <IconButton
              iconName="/static/images/pencil.outline.react.svg"
              size="12"
              onClick={() => console.log("onclick")}
            />
          </div>
        </div>
        <div className="row">
          <Text as="div" color="#A3A9AE" className="label">
            {t("Common:Password")}
          </Text>
          <div className="field">
            <Text fontWeight={600}>********</Text>
            <IconButton
              iconName="/static/images/pencil.outline.react.svg"
              size="12"
              onClick={() => console.log("onclick")}
            />
          </div>
        </div>
        <div className="row">
          <Text as="div" color="#A3A9AE" className="label">
            {t("Common:Language")}
          </Text>
          <LanguagesCombo
            profile={profile}
            cultureName={cultureName}
            currentCulture={currentCulture}
            culture={culture}
            cultureNames={cultureNames}
          />
        </div>
      </StyledInfo>
    </StyledWrapper>
  );
};

export default withCultureNames(MainProfile);
