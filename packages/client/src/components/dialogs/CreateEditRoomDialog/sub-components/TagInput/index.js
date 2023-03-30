import React, { useState, useRef } from "react";
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

  ${({ hasTags }) => !hasTags && "margin-bottom: -8px"}
`;

const TagInput = ({ t, tagHandler, setIsScrollLocked, isDisabled }) => {
  const inputRef = useRef();
  const [tagInput, setTagInput] = useState("");
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const onTagInputChange = (e) => {
    const text = e.target.value;

    if (text.trim().length > 0 && !isDropdownOpen) {
      openDropdown();
    } else if (text.length === 0 && isDropdownOpen) {
      closeDropdown();
    }

    setTagInput(text);
  };

  const handleFocus = (event) => {
    const text = event.target.value;
    if (text.trim().length > 0) {
      openDropdown();
    }
  };

  const openDropdown = () => {
    if (isDisabled) return;
    setIsScrollLocked(true);
    setIsDropdownOpen(true);
  };

  const closeDropdown = () => {
    setIsScrollLocked(false);
    setIsDropdownOpen(false);
  };

  const handleKeyDown = (event) => {
    const keyCode = event.code;

    const isAcceptableEvents =
      keyCode === "ArrowUp" || keyCode === "ArrowDown" || keyCode === "Enter";

    if (isAcceptableEvents && isDropdownOpen) return;

    event.stopPropagation();
  };

  return (
    <StyledTagInput
      className="set_room_params-input set_room_params-tag_input"
      hasTags={!!tagHandler.tags.length}
    >
      <InputParam
        ref={inputRef}
        id="shared_tags-input"
        title={`${t("Common:Tags")}:`}
        placeholder={t("TagsPlaceholder")}
        value={tagInput}
        onChange={onTagInputChange}
        onBlur={closeDropdown}
        onFocus={handleFocus}
        isDisabled={isDisabled}
        onKeyDown={handleKeyDown}
      />

      <TagDropdown
        inputRef={inputRef}
        open={isDropdownOpen}
        tagHandler={tagHandler}
        tagInputValue={tagInput}
        setTagInputValue={setTagInput}
        createTagLabel={t("CreateTagOption")}
      />

      <TagList
        tagHandler={tagHandler}
        defaultTagLabel={""}
        isDisabled={isDisabled}
      />
    </StyledTagInput>
  );
};

export default TagInput;
