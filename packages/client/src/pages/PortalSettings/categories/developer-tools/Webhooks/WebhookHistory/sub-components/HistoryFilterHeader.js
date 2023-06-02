import React, { useState } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { Base } from "@docspace/components/themes";
import FilterReactSvrUrl from "PUBLIC_DIR/images/filter.react.svg?url";
import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";

import { useParams } from "react-router-dom";
import FilterDialog from "./FilterDialog";
import StatusBar from "./StatusBar";
import { useTranslation } from "react-i18next";

const ListHeader = styled.header`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1px;
`;

const ListHeading = styled(Text)`
  line-height: 22px;
  font-weight: 700;
  margin: 0;
`;

const FilterButton = styled.div`
  position: relative;
  display: flex;
  box-sizing: border-box;
  flex-direction: row;
  justify-content: center;
  align-items: center;

  box-sizing: border-box;

  width: 32px;
  height: 32px;

  z-index: 202;

  border: 1px solid;
  border-color: ${(props) => (props.theme.isBase ? "#d0d5da" : "rgb(71, 71, 71)")};
  border-radius: 3px;
  cursor: pointer;

  svg {
    cursor: pointer;
  }

  :hover {
    border-color: #a3a9ae;
    svg {
      path {
        fill: ${(props) => props.theme.iconButton.hoverColor};
      }
    }
  }

  span {
    z-index: 203;
    width: 8px;
    height: 8px;
    background-color: #4781d1;
    border-radius: 50%;
    position: absolute;
    bottom: -2px;
    right: -2px;
  }
`;

FilterButton.defaultProps = { theme: Base };

const HistoryFilterHeader = (props) => {
  const { applyFilters, historyFilters } = props;
  const { t } = useTranslation(["Webhooks"]);
  const { id } = useParams();

  const [isFiltersVisible, setIsFiltersVisible] = useState(false);

  const openFiltersModal = () => {
    setIsFiltersVisible(true);
  };

  const closeFiltersModal = () => {
    setIsFiltersVisible(false);
  };

  return (
    <div>
      <ListHeader>
        <ListHeading fontWeight={700} fontSize="16px">
          {t("Webhook")} {id}
        </ListHeading>

        <FilterButton onClick={openFiltersModal}>
          <IconButton iconName={FilterReactSvrUrl} size={16} />
          <span hidden={historyFilters === null}></span>
        </FilterButton>
      </ListHeader>
      {historyFilters !== null && <StatusBar applyFilters={applyFilters} />}
      <FilterDialog
        visible={isFiltersVisible}
        closeModal={closeFiltersModal}
        applyFilters={applyFilters}
      />
    </div>
  );
};

export default inject(({ webhooksStore }) => {
  const { historyFilters } = webhooksStore;
  return {
    historyFilters,
  };
})(observer(HistoryFilterHeader));
