import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import RoomTypeDropdown from "./RoomTypeDropdown";
import ThirdPartyStorage from "./ThirdPartyStorage";
import TagInput from "./TagInput";
import RoomType from "./RoomType";

import PermanentSettings from "./PermanentSettings";
import InputParam from "./Params/InputParam";
import IsPrivateParam from "./IsPrivateParam";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";
import { getRoomTypeDefaultTagTranslation } from "../data";

import ImageEditor from "@docspace/components/ImageEditor";
import PreviewTile from "@docspace/components/ImageEditor/PreviewTile";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 22px;

  .icon-editor {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    justify-content: start;
    gap: 16px;
  }
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setIsOauthWindowOpen,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
  isEdit,
  isDisabled,
  isValidTitle,
  setIsValidTitle,
}) => {
  const [previewIcon, setPreviewIcon] = React.useState(null);

  const onChangeName = (e) => {
    setIsValidTitle(true);
    setRoomParams({ ...roomParams, title: e.target.value });
  };

  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  const onChangeStorageLocation = (storageLocation) =>
    setRoomParams({ ...roomParams, storageLocation });

  const onChangeIcon = (icon) => setRoomParams({ ...roomParams, icon: icon });

  return (
    <StyledSetRoomParams>
      {isEdit ? (
        <RoomType t={t} roomType={roomParams.type} type="displayItem" />
      ) : (
        <RoomTypeDropdown
          t={t}
          currentRoomType={roomParams.type}
          setRoomType={setRoomType}
          setIsScrollLocked={setIsScrollLocked}
          isDisabled={isDisabled}
        />
      )}

      {isEdit && (
        <PermanentSettings
          t={t}
          title={roomParams.title}
          isThirdparty={roomParams.isThirdparty}
          storageLocation={roomParams.storageLocation}
          isPrivate={roomParams.isPrivate}
          isDisabled={isDisabled}
        />
      )}

      <InputParam
        id="shared_room-name"
        title={`${t("Common:Name")}:`}
        placeholder={t("Common:EnterName")}
        value={roomParams.title}
        onChange={onChangeName}
        isDisabled={isDisabled}
        isValidTitle={isValidTitle}
        errorMessage={t("Common:RequiredField")}
      />

      <TagInput
        t={t}
        tagHandler={tagHandler}
        setIsScrollLocked={setIsScrollLocked}
        isDisabled={isDisabled}
      />

      {/* {!isEdit && (
        <IsPrivateParam
          t={t}
          isPrivate={roomParams.isPrivate}
          onChangeIsPrivate={onChangeIsPrivate}
        />
      )} */}

      {!isEdit && (
        <ThirdPartyStorage
          t={t}
          roomTitle={roomParams.title}
          storageLocation={roomParams.storageLocation}
          onChangeStorageLocation={onChangeStorageLocation}
          setIsScrollLocked={setIsScrollLocked}
          setIsOauthWindowOpen={setIsOauthWindowOpen}
          isDisabled={isDisabled}
        />
      )}

      <ImageEditor
        t={t}
        isDisabled={isDisabled}
        image={roomParams.icon}
        setPreview={setPreviewIcon}
        onChangeImage={onChangeIcon}
        classNameWrapperImageCropper={"icon-editor"}
        Preview={
          <PreviewTile
            t={t}
            title={roomParams.title || t("Files:NewRoom")}
            previewIcon={previewIcon}
            tags={roomParams.tags.map((tag) => tag.name)}
            isDisabled={isDisabled}
            defaultTagLabel={getRoomTypeDefaultTagTranslation(
              roomParams.type,
              t
            )}
          />
        }
      />
    </StyledSetRoomParams>
  );
};

export default withTranslation(["CreateEditRoomDialog"])(
  withLoader(SetRoomParams)(<Loaders.SetRoomParamsLoader />)
);
