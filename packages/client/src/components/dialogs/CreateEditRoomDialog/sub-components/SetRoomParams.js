import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import { roomTypes } from "../data";
import RoomTypeDropdown from "./RoomTypeDropdown";
import ThirdPartyStorage from "./ThirdPartyStorage";
import TagInput from "./TagInput";
import RoomType from "./RoomType";
import IconEditor from "./IconEditor";
import PermanentSettings from "./PermanentSettings";
import InputParam from "./Params/InputParam";
import IsPrivateParam from "./IsPrivateParam";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 20px;
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
}) => {
  const onChangeName = (e) =>
    setRoomParams({ ...roomParams, title: e.target.value });

  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  const onChangeStorageLocation = (storageLocation) =>
    setRoomParams({ ...roomParams, storageLocation });

  const onChangeIcon = (icon) => setRoomParams({ ...roomParams, icon: icon });

  const [currentRoomTypeData] = roomTypes.filter(
    (room) => room.type === roomParams.type
  );

  return (
    <StyledSetRoomParams>
      {isEdit ? (
        <RoomType t={t} room={currentRoomTypeData} type="displayItem" />
      ) : (
        <RoomTypeDropdown
          t={t}
          currentRoom={currentRoomTypeData}
          setRoomType={setRoomType}
          setIsScrollLocked={setIsScrollLocked}
        />
      )}

      {isEdit && (
        <PermanentSettings
          t={t}
          title={roomParams.title}
          isPrivate={roomParams.isPrivate}
          storageLocation={roomParams.storageLocation}
        />
      )}

      <InputParam
        id={"room-name"}
        title={`${t("Common:Name")}:`}
        placeholder={t("NamePlaceholder")}
        value={roomParams.title}
        onChange={onChangeName}
      />

      <TagInput
        t={t}
        tagHandler={tagHandler}
        currentRoomTypeData={currentRoomTypeData}
        setIsScrollLocked={setIsScrollLocked}
      />

      {!isEdit && (
        <IsPrivateParam
          t={t}
          isPrivate={roomParams.isPrivate}
          onChangeIsPrivate={onChangeIsPrivate}
        />
      )}

      {!isEdit && (
        <ThirdPartyStorage
          t={t}
          roomTitle={roomParams.title}
          storageLocation={roomParams.storageLocation}
          onChangeStorageLocation={onChangeStorageLocation}
          setIsScrollLocked={setIsScrollLocked}
          setIsOauthWindowOpen={setIsOauthWindowOpen}
        />
      )}

      <IconEditor
        t={t}
        roomType={roomParams.type}
        title={roomParams.title}
        tags={roomParams.tags}
        defaultTag={currentRoomTypeData.defaultTag}
        isPrivate={roomParams.isPrivate}
        icon={roomParams.icon}
        onChangeIcon={onChangeIcon}
      />
    </StyledSetRoomParams>
  );
};

export default withTranslation(["CreateEditRoomDialog"])(
  withLoader(SetRoomParams)(<Loaders.SetRoomParamsLoader />)
);
