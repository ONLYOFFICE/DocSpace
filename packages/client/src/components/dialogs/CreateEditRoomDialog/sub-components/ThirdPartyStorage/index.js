import Checkbox from "@docspace/components/checkbox";
import React from "react";
import styled from "styled-components";
import { StyledParam } from "../Params/StyledParam";

import ToggleParam from "../Params/ToggleParam";
import ThirpartyComboBox from "./ThirpartyComboBox";

import Toast from "@docspace/components/toast";
import toastrHelper from "@docspace/client/src/helpers/toastr";
import FolderInput from "./FolderInput";

const StyledThirdPartyStorage = styled(StyledParam)`
  flex-direction: column;
  gap: 12px;
`;

const ThirdPartyStorage = ({
  t,
  connectItems,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdpartyResponse,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,
  isThirdparty,
  onChangeIsThirdparty,
  storageLocation,
  setChangeStorageLocation,
  rememberThirdpartyStorage,
  onChangeRememberThirdpartyStorage,
  setIsScrollLocked,
  setIsOauthWindowOpen,
}) => {
  const checkForProviders = () => {
    if (connectItems.length) onChangeIsThirdparty();
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

  const onChangeProvider = (provider) =>
    setChangeStorageLocation({ ...storageLocation, provider });

  const onChangeFolderPath = (e) =>
    setChangeStorageLocation({
      ...storageLocation,
      storageFolderPath: e.target.value,
    });

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
          connectItems={connectItems}
          setConnectDialogVisible={setConnectDialogVisible}
          setRoomCreation={setRoomCreation}
          saveThirdpartyResponse={saveThirdpartyResponse}
          openConnectWindow={openConnectWindow}
          setConnectItem={setConnectItem}
          getOAuthToken={getOAuthToken}
          storageLocation={storageLocation}
          onChangeProvider={onChangeProvider}
          setChangeStorageLocation={setChangeStorageLocation}
          setIsScrollLocked={setIsScrollLocked}
          setIsOauthWindowOpen={setIsOauthWindowOpen}
        />
      )}

      {/* {isThirdparty && storageLocation.isConnected && ( */}
      <FolderInput
        value={storageLocation.storageFolderPath}
        onChangeFolderPath={onChangeFolderPath}
      />
      {/* )} */}

      {/* {isThirdparty && storageLocation.isConnected && ( */}
      <Checkbox
        className="thirdparty-checkbox"
        label={t("ThirdPartyStorageRememberChoice")}
        isChecked={rememberThirdpartyStorage}
        onChange={onChangeRememberThirdpartyStorage}
      />
      {/* )} */}
    </StyledThirdPartyStorage>
  );
};

export default ThirdPartyStorage;
