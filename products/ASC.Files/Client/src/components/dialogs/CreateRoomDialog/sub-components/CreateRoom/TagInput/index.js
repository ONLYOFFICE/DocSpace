import React, { useState, useRef } from "react";
import styled from "styled-components";

import Label from "@appserver/components/label";
import TextInput from "@appserver/components/text-input";
import TagList from "./TagList";
import DropDownItem from "@appserver/components/drop-down-item";
import {
  StyledDropDown,
  StyledDropDownWrapper,
} from "../../common/StyledDropdown";

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
    max-width: 100%;
    position: relative;
  }
`;

const TagInput = ({ t, tagHandler, setIsScrollLocked }) => {
  const [tagInput, setTagInput] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const chosenTags = tagHandler.tags.map((tag) => tag.name);
  const fetchedTags = tagHandler.fetchedTags;
  const filteredFetchedTags = fetchedTags.filter((tag) =>
    tag.toLowerCase().includes(tagInput.toLowerCase())
  );
  const tagsForDropdown = filteredFetchedTags.filter(
    (tag) => !chosenTags.includes(tag)
  );

  const onTagInputChange = (e) => setTagInput(e.target.value);
  const preventDefault = (e) => e.preventDefault();

  const openDropdown = () => {
    setIsScrollLocked(true);
    setIsOpen(true);
  };
  const closeDropdown = () => {
    setIsScrollLocked(false);
    setIsOpen(false);
  };

  const tagsInputElement = document.getElementById("tags-input");

  const onClickOutside = () => {
    tagsInputElement.blur();
  };
  const addNewTag = () => {
    tagHandler.addTag(tagInput);
    tagsInputElement.blur();
  };
  const addFetchedTag = (name) => {
    tagHandler.addTag(name);
    tagsInputElement.blur();
  };

  const dropdownRef = useRef(null);

  let dropdownItems = tagsForDropdown.map((tag, i) => (
    <DropDownItem
      className="dropdown-item"
      height={32}
      heightTablet={32}
      key={i}
      label={tag}
      onClick={() => {
        addFetchedTag(tag);
        console.log(tag);
      }}
    />
  ));

  if (
    tagInput &&
    ![...tagsForDropdown, ...chosenTags].find((tag) => tagInput === tag)
  ) {
    const dropdownItemNewTag = (
      <DropDownItem
        key={-2}
        className="dropdown-item"
        onMouseDown={preventDefault}
        onClick={addNewTag}
        label={`Create tag “${tagInput}”`}
        height={32}
        heightTablet={32}
      />
    );

    if (tagsForDropdown.length > 0) {
      dropdownItems = [
        dropdownItemNewTag,
        <DropDownItem height={7} heightTablet={7} key={-1} isSeparator />,
        ...dropdownItems,
      ];
    } else {
      dropdownItems = [dropdownItemNewTag, ...dropdownItems];
    }
  }

  return (
    <StyledTagInput className="set_room_params-input set_room_params-tag_input">
      <div className="set_room_params-tag_input-label_wrapper">
        <Label
          className="set_room_params-tag_input-label_wrapper-label"
          display="display"
          htmlFor="tags-input"
          text={`${t("Tags")}:`}
          title="Fill the first name field"
        />
      </div>
      <TextInput
        id="tags-input"
        value={tagInput}
        onChange={onTagInputChange}
        onFocus={openDropdown}
        onBlur={closeDropdown}
        scale
        placeholder={t("Add a tag")}
        tabIndex={2}
      />

      <StyledDropDownWrapper
        className="dropdown-content-wrapper"
        ref={dropdownRef}
        onMouseDown={preventDefault}
      >
        <StyledDropDown
          className="dropdown-content"
          open={isOpen}
          forwardedRef={dropdownRef}
          clickOutsideAction={onClickOutside}
          maxHeight={158}
          showDisabledItems={false}
          // onMouseDown={preventDefault}
        >
          {dropdownItems}
        </StyledDropDown>
      </StyledDropDownWrapper>
      <TagList t={t} tagHandler={tagHandler} />
    </StyledTagInput>
  );
};

export default TagInput;
