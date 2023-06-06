import React from "react";
import styled from "styled-components";

import EmptyFilterImg from "PUBLIC_DIR/images/empty_filter.react.svg?url";
import EmptyFilterDarkImg from "PUBLIC_DIR/images/empty_filter_dark.react.svg?url";
import ClearEmptyFilterIcon from "PUBLIC_DIR/images/clear.empty.filter.svg?url";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

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
  const { applyFilters, formatFilters, clearHistoryFilters, theme } = props;
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
        <img src={theme.isBase ? EmptyFilterImg : EmptyFilterDarkImg} alt="Empty filter" />
        <div className="emptyFilterText">
          <Text fontSize="16px" fontWeight={700} as="p" className="emptyFilterHeading">
            {t("Common:NotFoundTitle")}
          </Text>
          <Text fontSize="12px" color={theme.isBase ? "#555F65" : "rgba(255, 255, 255, 0.6)"}>
            {t("NoResultsMatched")}
          </Text>
          <span className="clearFilter" onClick={clearFilters}>
            <img src={ClearEmptyFilterIcon} alt={t("ClearFilter")} className="clearFilterIcon" />
            <Link
              color={theme.isBase ? "#657077" : "inherit"}
              isHovered
              fontWeight={600}
              type="action">
              {t("Common:ClearFilter")}
            </Link>
          </span>
        </div>
      </EmptyFilterContent>
    </EmptyFilterWrapper>
  );
};

export default inject(({ webhooksStore, auth }) => {
  const { formatFilters, clearHistoryFilters } = webhooksStore;
  const { theme } = auth.settingsStore;

  return { formatFilters, clearHistoryFilters, theme };
})(observer(EmptyFilter));
