import React from "react";

import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import Loader from "@appserver/components/loader";
import Loaders from "@appserver/common/components/Loaders";
const Option = ({
  style,
  isMultiSelect,
  index,
  isChecked,
  avatarUrl,
  label,
  keyProp,
  onOptionChange,
  onLinkClick,
  isLoader,
  countLoaderRows,
}) => {
  const onOptionChangeAction = React.useCallback(() => {
    onOptionChange && onOptionChange(index, isChecked);
  }, [onOptionChange, index, isChecked]);

  const onLinkClickAction = React.useCallback(() => {
    onLinkClick && onLinkClick(index);
  }, [onLinkClick, index]);

  return isLoader ? (
    <div style={style}>
      <div key="loader" className="option-loader">
        <Loaders.ListLoader withoutFirstRectangle count={countLoaderRows} />
      </div>
    </div>
  ) : isMultiSelect ? (
    <div
      style={style}
      className="row-option"
      value={`${index}`}
      name={`selector-row-option-${index}`}
      onClick={onOptionChangeAction}
    >
      <div className="option-info">
        <Avatar
          className="option-avatar"
          role="user"
          size="min"
          source={avatarUrl}
          userName={label}
        />
        <Text
          className="option-text"
          truncate={true}
          noSelect={true}
          fontSize="14px"
        >
          {label}
        </Text>
      </div>
      <Checkbox
        id={keyProp}
        value={`${index}`}
        isChecked={isChecked}
        className="option-checkbox"
      />
    </div>
  ) : (
    <div
      key={keyProp}
      style={style}
      className="row-option"
      data-index={index}
      name={`selector-row-option-${index}`}
      onClick={onLinkClickAction}
    >
      <div className="option-info">
        <Avatar
          className="option-avatar"
          role="user"
          size="min"
          source={avatarUrl}
          userName={label}
        />
        <Text
          className="option-text"
          truncate={true}
          noSelect={true}
          fontSize="14px"
        >
          {label}
        </Text>
      </div>
    </div>
  );
};

export default React.memo(Option);
