import React from "react";
import PropTypes from "prop-types";
import Tag from "@docspace/components/tag";

import StyledTags from "./StyledTags";

const Tags = ({ id, className, style, tags, columnCount, onSelectTag }) => {
  const [renderedTags, setRenderedTags] = React.useState(null);

  const tagsRef = React.useRef(null);

  const updateRenderedTags = React.useCallback(() => {
    if (tags && tagsRef) {
      if (!columnCount) return;

      const newTags = [];
      const containerWidth = tagsRef.current.offsetWidth;

      if (tags.length === 1) {
        if (tags[0]?.isDefault) {
          const tag = { ...tags[0], maxWidth: `100%` };
          newTags.push(tag);
        } else if (tags[0]?.isThirdParty) {
          const tag = { ...tags[0], maxWidth: `36px` };
          newTags.push(tag);
        } else {
          const tag = { label: tags[0].label || tags[0], maxWidth: `100%` };
          newTags.push(tag);
        }

        return setRenderedTags(newTags);
      }

      if (
        columnCount >= tags.length ||
        (tags.length === 2 && tags[0]?.isThirdParty && tags[1]?.isDefault)
      ) {
        const thirdPartyTagCount = tags[0]?.isThirdParty ? 1 : 0;

        const currentTagMaxWidth =
          (containerWidth -
            thirdPartyTagCount * 40 -
            (tags.length - thirdPartyTagCount) * 4) /
          (tags.length - thirdPartyTagCount);

        const maxWidthPercent = Math.floor(
          (currentTagMaxWidth / containerWidth) * 100
        );

        for (let i = 0; i < tags.length; i++) {
          if (tags[i]?.isThirdParty) {
            const tag = { ...tags[i], maxWidth: `36px` };
            newTags.push(tag);
          } else if (tags[i]?.isDefault) {
            const tag = { ...tags[i], maxWidth: `${maxWidthPercent}%` };
            newTags.push(tag);
          } else {
            const tag = { label: tags[i], maxWidth: `${maxWidthPercent}%` };
            newTags.push(tag);
          }
        }
      } else {
        const tagWithDropdown = {
          key: "selector",
          advancedOptions: tags.slice(columnCount, tags.length),
        };

        const currentTagMaxWidth =
          (containerWidth - columnCount * 4 - 35) / columnCount;

        const maxWidthPercent = Math.floor(
          (currentTagMaxWidth / containerWidth) * 100
        );

        if (columnCount !== 0) {
          for (let i = 0; i < columnCount; i++) {
            if (tags[i]?.isThirdParty) {
              const tag = { ...tags[i], maxWidth: `36px` };
              newTags.push(tag);
            } else if (tags[i]?.isDefault) {
              const tag = { ...tags[i], maxWidth: `${maxWidthPercent}%` };
              newTags.push(tag);
            } else {
              const tag = { label: tags[i], maxWidth: `${maxWidthPercent}%` };
              newTags.push(tag);
            }
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
            key={`${tag.label}_${index}`}
            label={tag.label}
            advancedOptions={tag.advancedOptions}
            tagMaxWidth={tag.maxWidth}
            tagMinWidth={tag.minWidth}
            isNewTag={false}
            onClick={onSelectTag}
            isLast={index === renderedTags.length - 1}
            {...tag}
          />
        ))}
    </StyledTags>
  );
};

Tag.propTypes = {
  /** Accepts the tags */
  tags: PropTypes.array,
  /** Accepts the tag column count */
  columnCount: PropTypes.number,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Accepts the function that is called when the tag is selected */
  onSelectTag: PropTypes.func,
};

export default Tags;
