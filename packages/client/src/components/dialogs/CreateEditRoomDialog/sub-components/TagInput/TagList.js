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
    padding: 6px 8px;
    border-radius: 3px;
    margin: 0;

    .tag-icon {
      margin-left: 10px;
    }
  }
`;

const TagList = ({ defaultTagLabel, tagHandler }) => {
  const { tags } = tagHandler;

  return (
    <StyledTagList className="set_room_params-tag_input-tag_list">
      {tags.length ? (
        tags.map((tag) => (
          <Tag
            key={tag.id}
            className="set_room_params-tag_input-tag"
            tag="script"
            label={tag.name}
            isNewTag
            onDelete={() => {
              tagHandler.deleteTag(tag.id);
            }}
          />
        ))
      ) : (
        <Tag
          className="set_room_params-tag_input-tag"
          tag="script"
          label={defaultTagLabel}
          isDisabled
        />
      )}
    </StyledTagList>
  );
};

export default TagList;
