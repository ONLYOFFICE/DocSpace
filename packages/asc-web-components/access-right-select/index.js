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

const AccessRightSelect = ({ accessRightsList, quotaList, ...props }) => {
  const [currentItem, setCurrentItem] = useState(accessRightsList[6]);

  const formatToAccessRightItem = (data) => {
    return (
      <>
        {data.map((it) => {
          const itQuota = quotaList.find(
            (elem) => elem.accessRightKey == it.key
          );
          return it.isSeparator ? (
            <DropDownItem isSeparator />
          ) : (
            <DropDownItem key={it.key} onClick={() => setCurrentItem(it)}>
              <StyledAccessRightItem>
                <StyledAccessRightItemIcon src={it.icon} />
                <StyledAccessRightItemContent>
                  <StyledAccessRightItemTitleAndBadge>
                    {it.title}
                    {itQuota && (
                      <Badge
                        label={itQuota.quota}
                        backgroundColor={itQuota.color}
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
        advancedOptions={formatToAccessRightItem(accessRightsList)}
        directionX="left"
        directionY="bottom"
        opened
        noBorder
        options={[]}
        scaled={false}
        selectedOption={{
          default: true,
          key: 0,
          label: currentItem?.title,
        }}
        size="content"
      />
    </StyledAccessRightWrapper>
  );
};

AccessRightSelect.propTypes = {
  /** List of rights */
  accessRightsList: PropTypes.arrayOf(PropTypes.object),
  /** List of quotas */
  quotaList: PropTypes.arrayOf(PropTypes.object),
};

export default AccessRightSelect;
