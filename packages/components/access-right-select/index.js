import React, { useState, useEffect, useCallback } from "react";
import PropTypes from "prop-types";
import DropDownItem from "../drop-down-item/index.js";
import Badge from "../badge/index.js";

import {
  StyledItemTitle,
  StyledItemContent,
  StyledItemIcon,
  StyledItemDescription,
  StyledItem,
  StyledWrapper,
} from "./styled-accessright.js";

const AccessRightSelect = ({
  accessOptions,
  onSelect,
  advancedOptions,
  selectedOption,
  ...props
}) => {
  const [currentItem, setCurrentItem] = useState(selectedOption);

  useEffect(() => {
    setCurrentItem(selectedOption);
  }, [selectedOption]);

  const onSelectCurrentItem = useCallback(
    (e) => {
      const key = e.currentTarget.dataset.key;
      const item = accessOptions.find((el) => {
        return el.key === key;
      });

      if (item) {
        setCurrentItem(item);
        onSelect && onSelect(item);
      }
    },
    [accessOptions, onSelect]
  );

  const formatToAccessRightItem = (data) => {
    const items = data.map((item) => {
      return item.isSeparator ? (
        <DropDownItem key={item.key} isSeparator />
      ) : (
        <DropDownItem
          key={item.key}
          data-key={item.key}
          onClick={onSelectCurrentItem}
        >
          <StyledItem>
            {item.icon && <StyledItemIcon src={item.icon} />}
            <StyledItemContent>
              <StyledItemTitle>
                {item.label}
                {item.quota && (
                  <Badge
                    label={item.quota}
                    backgroundColor={item.color}
                    fontSize="9px"
                    isPaidBadge
                  />
                )}
              </StyledItemTitle>
              <StyledItemDescription>{item.description}</StyledItemDescription>
            </StyledItemContent>
          </StyledItem>
        </DropDownItem>
      );
    });

    return <>{items}</>;
  };

  const formattedOptions = !!advancedOptions
    ? advancedOptions
    : formatToAccessRightItem(accessOptions);

  return (
    <StyledWrapper
      advancedOptions={formattedOptions}
      onSelect={onSelectCurrentItem}
      options={[]}
      selectedOption={{
        icon: currentItem?.icon,
        default: true,
        key: currentItem?.key,
        label: currentItem?.label,
      }}
      {...props}
    />
  );
};

AccessRightSelect.propTypes = {
  /** Will be triggered whenever an AccessRightSelect is selected option */
  onSelect: PropTypes.func,
  /** List of advanced options */
  advancedOptions: PropTypes.oneOfType([PropTypes.array, PropTypes.object]),
  /** List of access options */
  accessOptions: PropTypes.oneOfType([PropTypes.array, PropTypes.object])
    .isRequired,
  /** The option that is selected by default */
  selectedOption: PropTypes.object,
};

export default React.memo(AccessRightSelect);
