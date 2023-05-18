import React from "react";
import styled from "styled-components";

import EmptyFilterImg from "PUBLIC_DIR/images/empty_filter.react.svg?url";
import ClearEmptyFilterIcon from "PUBLIC_DIR/images/clear.empty.filter.svg?url";
import { Text } from "@docspace/components";
import { Link } from "@docspace/components";

import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

const EmptyFilterWrapper = styled.div`
  width: 100%;
  display: flex;
  justify-content: center;
  margin-top: 149px;
`;

const EmptyFilterContent = styled.div`
  display: flex;

  .emptyFilterText {
    margin-left: 40px;
  }

  .clearFilter {
    display: block;
    margin-top: 26px;
    cursor: pointer;
  }

  .clearFilterIcon {
    margin-right: 8px;
  }

  .emptyFilterHeading {
    margin-bottom: 8px;
  }
`;

const EmptyFilter = (props) => {
  const { applyFilters, formatFilters, clearHistoryFilters } = props;
  const { t } = useTranslation(["Webhooks", "Common"]);

  const clearFilters = () => {
    clearHistoryFilters(null);
    applyFilters(
      formatFilters({
        deliveryDate: null,
        status: [],
      }),
    );
  };

  return (
    <EmptyFilterWrapper>
      <EmptyFilterContent>
        <img src={EmptyFilterImg} alt="Empty filter" />
        <div className="emptyFilterText">
          <Text fontSize="16px" fontWeight={700} as="p" className="emptyFilterHeading">
            {t("NothingFound", { ns: "Webhooks" })}
          </Text>
          <Text fontSize="12px" color="#555F65">
            {t("NoResultsMatched", { ns: "Webhooks" })}
          </Text>
          <span className="clearFilter" onClick={clearFilters}>
            <img src={ClearEmptyFilterIcon} alt={t("ClearFilter", { ns: "Webhooks" })} className="clearFilterIcon" />
            <Link color="#657077" isHovered fontWeight={600}>
              {t("ClearFilter", { ns: "Common" })}
            </Link>
          </span>
        </div>
      </EmptyFilterContent>
    </EmptyFilterWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { formatFilters, clearHistoryFilters } = webhooksStore;

  return { formatFilters, clearHistoryFilters };
})(observer(EmptyFilter));
