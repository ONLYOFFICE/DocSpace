import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import moment from "moment";

import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";

import Button from "@docspace/components/button";
import DeliveryDatePicker from "./DeliveryDatePicker";
import StatusPicker from "./StatusPicker";

import { useParams, useNavigate } from "react-router-dom";

import { useTranslation } from "react-i18next";

import { Base } from "@docspace/components/themes";

const DialogBodyWrapper = styled.div`
  margin-top: -4px;
`;

const Footer = styled.div`
  width: 100%;
  display: flex;

  button {
    width: 100%;
  }
  button:first-of-type {
    margin-right: 10px;
  }
`;

const Separator = styled.hr`
  border-top: 1px solid;
  border-color: ${(props) => (props.theme.isBase ? "#eceef1" : "#474747")};
  margin-bottom: 14px;
`;

Separator.defaultProps = { theme: Base };

const constructUrl = (baseUrl, filters) => {
  const url = new URL(baseUrl, "http://127.0.0.1:8092/");
  url.searchParams.append("deliveryDate", filters.deliveryDate?.format("YYYY-MM-DD") || null);
  url.searchParams.append("deliveryFrom", filters.deliveryFrom.format("HH:mm"));
  url.searchParams.append("deliveryTo", filters.deliveryTo.format("HH:mm"));
  url.searchParams.append("status", JSON.stringify(filters.status));

  return url.pathname + url.search;
};

function areArraysEqual(array1, array2) {
  return array1.length === array2.length && array1.every((val, index) => val === array2[index]);
}

const FilterDialog = (props) => {
  const {
    visible,
    closeModal,
    applyFilters,
    formatFilters,
    setHistoryFilters,
    historyFilters,
  } = props;
  const { t } = useTranslation(["Webhooks", "Files", "Common"]);
  const { id } = useParams();
  const navigate = useNavigate();

  const [filters, setFilters] = useState({
    deliveryDate: null,
    deliveryFrom: moment().startOf("day"),
    deliveryTo: moment().endOf("day"),
    status: [],
  });

  const [isApplied, setIsApplied] = useState(false);

  const [isLoaded, setIsLoaded] = useState(false);

  const handleApplyFilters = () => {
    if (filters.deliveryTo > filters.deliveryFrom) {
      const params = formatFilters(filters);

      setHistoryFilters(filters);
      setIsApplied(true);

      applyFilters(params);
      closeModal();
    }
  };

  useEffect(() => {
    if (historyFilters === null) {
      if (filters.deliveryDate !== null || filters.status.length > 0) {
        setFilters({
          deliveryDate: null,
          deliveryFrom: moment().startOf("day"),
          deliveryTo: moment().endOf("day"),
          status: [],
        });
      }
      isLoaded && navigate(`/portal-settings/developer-tools/webhooks/${id}`);
    } else {
      setFilters(historyFilters);
      setIsApplied(true);
      navigate(constructUrl(`/portal-settings/developer-tools/webhooks/${id}`, historyFilters));
    }
    setIsLoaded(true);
  }, [historyFilters, visible]);

  const areFiltersChanged =
    historyFilters !== null
      ? areArraysEqual(filters.status, historyFilters.status) &&
        filters.deliveryDate === historyFilters?.deliveryDate &&
        filters.deliveryFrom === historyFilters.deliveryFrom &&
        filters.deliveryTo === historyFilters.deliveryTo
      : filters.deliveryDate === null && filters.status.length === 0;

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={closeModal} displayType="aside">
      <ModalDialog.Header>{t("Files:Filter")}</ModalDialog.Header>
      <ModalDialog.Body>
        <DialogBodyWrapper>
          <DeliveryDatePicker
            isApplied={isApplied}
            setIsApplied={setIsApplied}
            filters={filters}
            setFilters={setFilters}
          />
          <Separator />
          <StatusPicker filters={filters} setFilters={setFilters} />
          <Separator />
        </DialogBodyWrapper>
      </ModalDialog.Body>
      {!areFiltersChanged && (
        <ModalDialog.Footer>
          <Footer>
            <Button
              className="apply-button"
              label={t("Common:ApplyButton")}
              size="normal"
              primary={true}
              onClick={handleApplyFilters}
            />
            <Button
              className="cancel-button"
              label={t("Common:CancelButton")}
              size="normal"
              onClick={closeModal}
            />
          </Footer>
        </ModalDialog.Footer>
      )}
    </ModalDialog>
  );
};

export default inject(({ webhooksStore }) => {
  const { formatFilters, setHistoryFilters, historyFilters } = webhooksStore;

  return { formatFilters, setHistoryFilters, historyFilters };
})(observer(FilterDialog));
