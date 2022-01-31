import React, { useState } from "react";
import PropTypes from "prop-types";
import ComboBox from "../combobox/index.js";
import DropDownItem from "../drop-down-item/index.js";
import Badge from "../badge/index.js";

import {
  StyledAccessRightWrapper,
  StyledAccessRightIcon,
  StyledAccessRightDescriptionItem,
  StyledAccessRightItem,
  StyledAccessRightItemIcon,
  StyledAccessRightItemContent,
  StyledAccessRightItemTitleAndBadge,
} from "./styled-accessright.js";

const AccessRightSelect = ({ options, onSelect, selectedOption, ...props }) => {
  const [currentItem, setCurrentItem] = useState(selectedOption);

  function onSelectCurrentItem(e) {
    const key = e.currentTarget.dataset.key;
    const item = options.find((el) => {
      return el.key === key;
    });
    if (item) {
      setCurrentItem(item);
      onSelect && onSelect(item);
    }
  }

  const formatToAccessRightItem = (data) => {
    return (
      <>
        {data.map((it) => {
          return it.isSeparator ? (
            <DropDownItem isSeparator />
          ) : (
            <DropDownItem
              key={it.key}
              data-key={it.key}
              onClick={onSelectCurrentItem}
            >
              <StyledAccessRightItem>
                <StyledAccessRightItemIcon src={it.icon} />
                <StyledAccessRightItemContent>
                  <StyledAccessRightItemTitleAndBadge>
                    {it.title}
                    {it.quota && (
                      <Badge
                        label={it.quota}
                        backgroundColor={it.color}
                        fontSize="8px"
                      />
                    )}
                  </StyledAccessRightItemTitleAndBadge>
                  <StyledAccessRightDescriptionItem>
                    {it.description}
                  </StyledAccessRightDescriptionItem>
                </StyledAccessRightItemContent>
              </StyledAccessRightItem>
            </DropDownItem>
          );
        })}
      </>
    );
  };

  return (
    <StyledAccessRightWrapper>
      <StyledAccessRightIcon src={currentItem?.icon} />
      <ComboBox
        advancedOptions={formatToAccessRightItem(options)}
        directionX="left"
        directionY="bottom"
        opened
        noBorder
        options={[]}
        scaled={false}
        selectedOption={{
          default: true,
          key: currentItem?.key,
          label: currentItem?.title,
        }}
        size="content"
      />
    </StyledAccessRightWrapper>
  );
};

AccessRightSelect.propTypes = {
  /** List of rights */
  options: PropTypes.arrayOf(PropTypes.object).isRequired,
  /** Will be triggered whenever an AccessRightSelect is selected option */
  onSelect: PropTypes.func,
  /** The option that is selected by default */
  selectedOption: PropTypes.object,
};

export default AccessRightSelect;
