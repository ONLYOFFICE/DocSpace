import React from "react";

import Tag from "@appserver/components/tag";

import StyledTags from "./StyledTags";

const tagMaxWidth = 160;
const Tags = ({ id, className, style, tags, columnCount }) => {
  const [renderedTags, setRenderedTags] = React.useState(null);

  const tagsRef = React.useRef(null);

  const updateRenderedTags = React.useCallback(() => {
    if (tags && tagsRef && columnCount) {
      const newTags = [];

      const containerWidth = tagsRef.current.offsetWidth;

      if (columnCount >= tags.length) {
        const currentTagMaxWidth =
          (containerWidth - tags.length * 4) / tags.length;

        const maxWidthPercent = Math.ceil(
          (currentTagMaxWidth / containerWidth) * 100
        );

        newTags.push(...tags);

        for (let i = 0; i < tags.length; i++) {
          newTags[i].maxWidth = `${maxWidthPercent}%`;
        }
      } else {
        if (tags.length === columnCount + 1) {
          const currentTagMaxWidth =
            (containerWidth - tags.length * 4) / tags.length;

          const maxWidthPercent = Math.ceil(
            (currentTagMaxWidth / containerWidth) * 100
          );

          newTags.push(...tags);

          for (let i = 0; i < tags.length; i++) {
            newTags[i].maxWidth = `${maxWidthPercent}%`;
          }
        } else {
          const tagWithDropdown = {
            key: "selector",
            advancedOptions: tags.slice(columnCount, tags.length),
          };

          const currentTagMaxWidth =
            (containerWidth - columnCount * 4 - 35) / columnCount;

          const maxWidthPercent = Math.ceil(
            (currentTagMaxWidth / containerWidth) * 100
          );

          newTags.push(...tags.slice(0, columnCount));
          newTags.push(tagWithDropdown);

          for (let i = 0; i < columnCount; i++) {
            newTags[i].maxWidth = `${maxWidthPercent}%`;
          }

          newTags[newTags.length - 1].maxWidth = `35px`;
        }
      }

      setRenderedTags(newTags);
    }
  }, [tags, tagsRef, columnCount]);

  React.useEffect(() => {
    updateRenderedTags();
  }, [tags, tagsRef, columnCount]);

  return (
    <StyledTags id={id} className={className} style={style} ref={tagsRef}>
      {renderedTags?.length > 0 ? (
        renderedTags.map((tag) => (
          <Tag
            key={tag.key}
            label={tag.label}
            advancedOptions={tag.advancedOptions}
            tagMaxWidth={tag.maxWidth}
            isNewTag={false}
          />
        ))
      ) : (
        <Tag key={"empty"} label={"No tag"} isDisabled={true} />
      )}
    </StyledTags>
  );
};

export default Tags;
