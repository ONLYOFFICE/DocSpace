import Checkbox from "@docspace/components/checkbox";
import React from "react";
import styled from "styled-components";
import { StyledParam } from "../Params/StyledParam";

import ToggleParam from "../Params/ToggleParam";
import ThirpartyComboBox from "./ThirpartyComboBox";

import Toast from "@docspace/components/toast";
import toastrHelper from "@docspace/client/src/helpers/toastr";

const StyledThirdPartyStorage = styled(StyledParam)`
  flex-direction: column;
  gap: 12px;
`;

const ThirdPartyStorage = ({
  t,
  providers,
  isThirdparty,
  onChangeIsThirdparty,
  storageLocation,
  setChangeStorageLocation,
  rememberThirdpartyStorage,
  onChangeRememberThirdpartyStorage,
  setIsScrollLocked,
}) => {
  const checkForProviders = () => {
    if (providers.length) onChangeIsThirdparty();
    else
      toastrHelper.warning(
        <div>
          <div>{t("ThirdPartyStorageNoStorageAlert")}</div>
          <a href="#">Third-party services</a>
        </div>,
        "Alert",
        5000,
        true,
        false
      );
  };

  return (
    <StyledThirdPartyStorage>
      {/* <div className="set_room_params-info">
        <div className="set_room_params-info-title">
          <Text className="set_room_params-info-title-text">
            {t("ThirdPartyStorageTitle")}
          </Text>
        </div>
        <div className="set_room_params-info-description">
          {t("ThirdPartyStorageDescription")}
        </div>
      </div> */}

      <ToggleParam
        title={t("ThirdPartyStorageTitle")}
        description={t("ThirdPartyStorageDescription")}
        isChecked={isThirdparty}
        onCheckedChange={checkForProviders}
      />

      {isThirdparty && (
        <ThirpartyComboBox
          t={t}
          providers={providers}
          storageLocation={storageLocation}
          setChangeStorageLocation={setChangeStorageLocation}
          setIsScrollLocked={setIsScrollLocked}
        />
      )}

      {isThirdparty && storageLocation && (
        <Checkbox
          className="thirdparty-checkbox"
          label={t("ThirdPartyStorageRememberChoice")}
          isChecked={rememberThirdpartyStorage}
          onChange={onChangeRememberThirdpartyStorage}
        />
      )}
    </StyledThirdPartyStorage>
  );
};

export default ThirdPartyStorage;
