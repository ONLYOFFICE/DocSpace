import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";

import {
  connectedCloudsTypeIcon as getProviderTypeIcon,
  connectedCloudsTypeTitleTranslation as getProviderTypeTitle,
} from "@docspace/client/src/helpers/filesUtils";

import PermanentSetting from "./PermanentSetting";

const StyledPermanentSettings = styled.div`
  display: ${(props) => (props.displayNone ? "none" : "flex")};
  flex-direction: row;
  gap: 8px;
  margin-top: -12px;
`;

const PermanentSettings = ({
  t,
  title,
  isThirdparty,
  storageLocation,
  isPrivate,
}) => {
  const createThirdpartyPath = () => {
    console.log(storageLocation);
    const path = storageLocation.parentId.split("|");
    path.shift();
    path.unshift(thirdpartyTitle);
    path.push(thirdpartyFolderName);
    return `(${path.join("/")})`;
  };

  const thirdpartyTitle = getProviderTypeTitle(storageLocation?.providerKey, t);
  const thirdpartyFolderName = isThirdparty ? storageLocation?.title : "";
  const thirdpartyPath = isThirdparty ? createThirdpartyPath() : "";

  return (
    <StyledPermanentSettings displayNone={!(isPrivate || isThirdparty)}>
      {isThirdparty && (
        <PermanentSetting
          type="storageLocation"
          isFull={!isPrivate}
          icon={storageLocation.iconSrc}
          title={thirdpartyTitle}
          content={
            <Trans
              i18nKey="ThirdPartyStoragePermanentSettingDescription"
              ns="CreateEditRoomDialog"
              t={t}
            >
              Files are stored in a third-party {{ thirdpartyTitle }} storage in
              the \"{{ thirdpartyFolderName }}\" folder.{" "}
              {/* <strong>{{ thirdpartyPath }}</strong>" */}
            </Trans>
          }
        />
      )}
      {isPrivate && (
        <PermanentSetting
          type="privacy"
          isFull={!storageLocation}
          icon={"images/security.svg"}
          title={"Private room"}
          content={`All files in this room will be encrypted`}
        />
      )}
    </StyledPermanentSettings>
  );
};

export default PermanentSettings;
