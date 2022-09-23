import React from "react";

import Tag from "@docspace/components/tag";

import StyledTags from "./StyledTags";

const Tags = ({ id, className, style, tags, columnCount, onSelectTag }) => {
  const [renderedTags, setRenderedTags] = React.useState(null);

  const tagsRef = React.useRef(null);

  const updateRenderedTags = React.useCallback(() => {
    if (tags && tagsRef) {
      const newTags = [];

      const containerWidth = tagsRef.current.offsetWidth;

      if (tags.length === 1) {
        const tag = { name: tags[0], maxWidth: `100%` };

        newTags.push(tag);

        return setRenderedTags(newTags);
      }

      if (columnCount >= tags.length) {
        const currentTagMaxWidth =
          (containerWidth - tags.length * 4) / tags.length;

        const maxWidthPercent = Math.ceil(
          (currentTagMaxWidth / containerWidth) * 100
        );

        for (let i = 0; i < tags.length; i++) {
          const tag = { name: tags[i], maxWidth: `${maxWidthPercent}%` };

          newTags.push(tag);
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

        if (columnCount !== 0) {
          for (let i = 0; i < columnCount; i++) {
            const tag = { name: tags[i], maxWidth: `${maxWidthPercent}%` };

            newTags.push(tag);
          }
        }

        newTags.push(tagWithDropdown);
        newTags[newTags.length - 1].maxWidth = `35px`;
      }

      setRenderedTags(newTags);
    }
  }, [tags, tagsRef, columnCount]);

  React.useEffect(() => {
    updateRenderedTags();
  }, [tags, tagsRef, columnCount]);

  return (
    <StyledTags id={id} className={className} style={style} ref={tagsRef}>
      {renderedTags?.length > 0 &&
        renderedTags.map((tag, index) => (
          <Tag
            key={`${tag.name}_${index}`}
            label={tag.name}
            advancedOptions={tag.advancedOptions}
            tagMaxWidth={tag.maxWidth}
            isNewTag={false}
            onClick={onSelectTag}
          />
        ))}
    </StyledTags>
  );
};

export default Tags;
