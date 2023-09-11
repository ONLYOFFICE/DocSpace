import Tag from "@docspace/components/tag";
import React from "react";
import styled from "styled-components";

const StyledTagList = styled.div`
  margin-top: 12px;
  display: flex;
  flex-direction: row;
  gap: 4px;
  flex-wrap: wrap;
  width: 100%;

  .set_room_params-tag_input-tag {
    background: ${(props) =>
      props.theme.createEditRoomDialog.tagInput.tagBackground};
    padding: 6px 8px;
    border-radius: 3px;
    margin: 0;

    :hover {
      background: ${(props) =>
        props.theme.createEditRoomDialog.tagInput.tagHoverBackground};
    }
    .tag-icon {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-right: 10px;`
          : `margin-left: 10px;`}
      svg {
        width: 10px;
        height: 10px;
      }
    }
  }
`;

const TagList = ({ defaultTagLabel, tagHandler, isDisabled }) => {
  const { tags } = tagHandler;

  const onDeleteAction = (id) => {
    if (isDisabled) return;
    tagHandler.deleteTag(id);
  };

  return (
    <StyledTagList className="set_room_params-tag_input-tag_list">
      {tags.map((tag) => (
        <Tag
          key={tag.id}
          className="set_room_params-tag_input-tag"
          tag="script"
          label={tag.name}
          isNewTag
          onDelete={() => {
            onDeleteAction(tag.id);
          }}
        />
      ))}
      {/* {tags.length ? (
        tags.map((tag) => (
          <Tag
            key={tag.id}
            className="set_room_params-tag_input-tag"
            tag="script"
            label={tag.name}
            isNewTag
            onDelete={() => {
              onDeleteAction(tag.id);
            }}
          />
        ))
      ) : (
        <Tag
          className="set_room_params-tag_input-tag"
          tag="script"
          label={defaultTagLabel}
          isDefault
        />
      )} */}
    </StyledTagList>
  );
};

export default TagList;
