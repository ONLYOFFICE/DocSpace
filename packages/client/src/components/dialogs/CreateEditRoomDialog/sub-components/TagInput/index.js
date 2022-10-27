import React, { useState } from "react";
import styled from "styled-components";

import TagList from "./TagList";

import InputParam from "../Params/InputParam";
import TagDropdown from "./TagDropdown";

const StyledTagInput = styled.div`
  .set_room_params-tag_input {
    &-label_wrapper {
      &-label {
        cursor: pointer;
        width: auto;
        display: inline-block;
      }
    }
  }

  .dropdown-content-wrapper {
    margin-bottom: -4px;
    max-width: 100%;
    position: relative;
  }
`;

const TagInput = ({
  t,
  tagHandler,
  currentRoomTypeData,
  setIsScrollLocked,
  isDisabled,
}) => {
  const [tagInput, setTagInput] = useState("");
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const onTagInputChange = (e) => setTagInput(e.target.value);

  const openDropdown = () => {
    if (isDisabled) return;
    setIsScrollLocked(true);
    setIsDropdownOpen(true);
  };

  const closeDropdown = () => {
    setIsScrollLocked(false);
    setIsDropdownOpen(false);
  };

  return (
    <StyledTagInput className="set_room_params-input set_room_params-tag_input">
      <InputParam
        id={"tags-input"}
        title={`${t("Common:Tags")}:`}
        placeholder={t("TagsPlaceholder")}
        value={tagInput}
        onChange={onTagInputChange}
        onFocus={openDropdown}
        onBlur={closeDropdown}
        isDisabled={isDisabled}
      />

      <TagDropdown
        open={isDropdownOpen}
        tagHandler={tagHandler}
        tagInputValue={tagInput}
        setTagInputValue={setTagInput}
        createTagLabel={t("CreateTagOption")}
      />

      <TagList
        tagHandler={tagHandler}
        defaultTagLabel={t(currentRoomTypeData.defaultTag)}
        isDisabled={isDisabled}
      />
    </StyledTagInput>
  );
};

export default TagInput;
