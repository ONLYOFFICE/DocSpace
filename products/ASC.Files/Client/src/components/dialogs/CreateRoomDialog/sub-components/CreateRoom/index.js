import React, { useState } from "react";
import styled, { css } from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";
import StorageLocation from "./StorageLocation";

import TextInput from "@appserver/components/text-input";
import Label from "@appserver/components/label";
import ToggleButton from "@appserver/components/toggle-button";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";
import AvatarEditor from "@appserver/components/avatar-editor";
import TagInput from "./TagInput";
import { StyledParam } from "../common/StyledParam";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 20px;

  .set_room_params-input {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }
`;

const StyledIconEditorWrapper = styled.div`
  .use_modal-avatar_editor_body {
    margin: 0;
  }

  .use_modal-buttons_wrapper {
    display: none;
  }
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
}) => {
  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  return (
    <StyledSetRoomParams>
      <RoomTypeDropdown
        t={t}
        currentRoomType={roomParams.type}
        setRoomType={setRoomType}
      />

      <div className="set_room_params-input">
        <Label
          display="display"
          htmlFor="room-name"
          text={`${t("Common:Name")}:`}
          title="Fill the first name field"
        />
        <TextInput
          id="room-name"
          value={roomParams.title}
          onChange={(e) =>
            setRoomParams({ ...roomParams, title: e.target.value })
          }
          scale
          placeholder="New room"
          tabIndex={1}
        />
      </div>

      <TagInput
        t={t}
        tagHandler={tagHandler}
        setIsScrollLocked={setIsScrollLocked}
      />

      <StyledParam isPrivate>
        <div className="set_room_params-info">
          <div className="set_room_params-info-title">
            <Text className="set_room_params-info-title-text">
              {t("MakeRoomPrivateTitle")}
            </Text>
            <HelpButton
              displayType="auto"
              className="set_room_params-info-title-help"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={t("MakeRoomPrivateDescription")}
              size={12}
            />
          </div>
          <div className="set_room_params-info-description">
            {t("MakeRoomPrivateDescription")}
          </div>
        </div>
        <ToggleButton
          className="set_room_params-toggle"
          isChecked={roomParams.isPrivate}
          onChange={onChangeIsPrivate}
        />
      </StyledParam>

      <StorageLocation
        t={t}
        roomParams={roomParams}
        setRoomParams={setRoomParams}
        setIsScrollLocked={setIsScrollLocked}
      />

      {/* <StyledIconEditorWrapper>
        <AvatarEditor useModalDialog={false}></AvatarEditor>
      </StyledIconEditorWrapper> */}
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
