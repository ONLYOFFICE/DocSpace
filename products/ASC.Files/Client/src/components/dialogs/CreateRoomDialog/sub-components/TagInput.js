import React, { useState, useRef } from "react";
import styled from "styled-components";

import Label from "@appserver/components/label";
import TextInput from "@appserver/components/text-input";
import TagList from "../views/CreateRoom/TagList";
import DropDownItem from "@appserver/components/drop-down-item";
import { StyledDropDown, StyledDropdownWrapper } from "./StyledDropdown";

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

// const StyledTagDropdown = styled.div`
//   display: ${(props) => (props.isOpen ? "flex" : "none")};
//   flex-direction: column;
//   padding: 4px 0;
//   background: #ffffff;
//   border: 1px solid #d0d5da;
//   box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
//   border-radius: 3px;
//   z-index: 400;
//   position: absolute;

//   width: 100%;

//   .dropdown-tag {
//     cursor: pointer;
//     box-sizing: border-box;
//     width: 100%;
//     padding: 6px 8px;
//     font-weight: 400;
//     font-size: 13px;
//     line-height: 20px;
//     outline: none;
//     color: #333333;
//     &:hover {
//       background: #f3f4f4;
//     }

//     &-separator {
//       height: 1px;
//       width: calc(100% - 24px);
//       box-sizing: border-box;
//       background: #eceef1;
//       margin: 3px 12px;
//     }
//   }
// `;

const TagInput = ({ t, tagHandler, setIsScrollLocked }) => {
  const [tagInput, setTagInput] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const fetchedTags = tagHandler.fetchedTags.filter((tag) =>
    tag.toLowerCase().includes(tagInput.toLowerCase())
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
  const addNewTag = () => {
    tagHandler.addTag(tagInput);
    tagsInputElement.blur();
  };
  const addFetchedTag = (name) => {
    tagHandler.addTag(name);
    tagsInputElement.blur();
  };

  const dropdownRef = useRef(null);

  const showCreateNewTag =
    tagInput && !fetchedTags.find((tag) => tagInput === tag);
  const showCreateNewTagSeparator = showCreateNewTag && fetchedTags.length > 0;
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

      <StyledDropdownWrapper
        className="dropdown-content-wrapper"
        ref={dropdownRef}
        onMouseDown={preventDefault}
      >
        <StyledDropDown
          className="dropdown-content"
          open={isOpen}
          //maxHeight={181}
          forwardedRef={dropdownRef}
          directionX={"left"}
          clickOutsideAction={closeDropdown}
          showDisabledItems={false}
          onMouseDown={preventDefault}
        >
          {showCreateNewTag && (
            <DropDownItem
              key={-2}
              className="dropdown-item"
              onMouseDown={preventDefault}
              onClick={addNewTag}
              label={`Create tag “${tagInput}”`}
            />
          )}

          {showCreateNewTagSeparator && <DropDownItem key={-1} isSeparator />}

          {fetchedTags.map((fetchedTag, i) => (
            <DropDownItem
              className="dropdown-item"
              key={i}
              label={fetchedTag}
              onClick={() => {
                addFetchedTag(fetchedTag);
                console.log(fetchedTag);
              }}
            />
          ))}
        </StyledDropDown>
      </StyledDropdownWrapper>

      {/* <div className="dropdown-content-wrapper">
        <StyledTagDropdown
          isOpen={isOpen}
          className="set_room_params-tag_input-dropdown"
        >
          {tagInput && !fetchedTags.find((tag) => tagInput === tag) && (
            <>
              <div
                className="dropdown-tag"
                onMouseDown={preventDefault}
                onClick={addNewTag}
              >
                Create tag “{tagInput}”
              </div>
              {fetchedTags.length > 0 && (
                <div className="dropdown-tag-separator"></div>
              )}
            </>
          )}
          {fetchedTags.map((fetchedTag, i) => (
            <div
              className="dropdown-tag"
              key={i}
              onMouseDown={preventDefault}
              onClick={() => addFetchedTag(fetchedTag)}
            >
              {fetchedTag}
            </div>
          ))}
        </StyledTagDropdown>
      </div> */}

      <TagList t={t} tagHandler={tagHandler} />
    </StyledTagInput>
  );
};

export default TagInput;
