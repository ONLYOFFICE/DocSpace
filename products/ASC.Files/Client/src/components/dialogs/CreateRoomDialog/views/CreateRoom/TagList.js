import Tag from "@appserver/components/tag";
import React from "react";
import styled from "styled-components";

const StyledTagList = styled.div`
  margin-top: 2px;
  display: flex;
  flex-direction: row;
  gap: 4px;
  width: 100px;
  .set_room_params-tag_input-tag {
    padding: 6px 8px;
    border-radius: 3px;

    .tag-icon {
      margin-left: 10px;
    }
  }
`;

const TagList = ({ t, tagHandler }) => {
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
          label={t("No tags")}
          isDisabled
        />
      )}
    </StyledTagList>
  );
};

export default TagList;
