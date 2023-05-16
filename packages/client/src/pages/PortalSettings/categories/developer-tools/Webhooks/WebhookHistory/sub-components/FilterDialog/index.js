import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import moment from "moment";

import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";

import { Button } from "@docspace/components";
import DeliveryDatePicker from "./DeliveryDatePicker";
import StatusPicker from "./StatusPicker";

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

const Selectors = styled.div`
  position: relative;
  margin-top: 12px;
  margin-bottom: 16px;
  .calendar {
    position: absolute;
  }
`;

const Separator = styled.hr`
  border-top: 1px solid #eceef1;
  margin-bottom: 16px;
`;

const FilterDialog = (props) => {
  const { visible, closeModal, applyFilters, formatFilters, setHistoryFilters } = props;

  const [filters, setFilters] = useState({
    deliveryDate: null,
    deliveryFrom: moment().startOf("day"),
    deliveryTo: moment().endOf("day"),
    status: [],
  });

  const [isApplied, setIsApplied] = useState(false);

  const handleApplyFilters = () => {
    if (filters.deliveryTo > filters.deliveryFrom) {
      const params = formatFilters(filters);

      setHistoryFilters(filters);
      setIsApplied(true);

      applyFilters(params);
      closeModal();
    }
  };

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={closeModal} displayType="aside">
      <ModalDialog.Header>Search options</ModalDialog.Header>
      <ModalDialog.Body>
        <DeliveryDatePicker
          Selectors={Selectors}
          isApplied={isApplied}
          setIsApplied={setIsApplied}
          filters={filters}
          setFilters={setFilters}
        />
        <Separator />
        <StatusPicker Selectors={Selectors} filters={filters} setFilters={setFilters} />
        <Separator />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button label="Apply" size="normal" primary={true} onClick={handleApplyFilters} />
          <Button label="Cancel" size="normal" onClick={closeModal} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ webhooksStore }) => {
  const { formatFilters, setHistoryFilters } = webhooksStore;

  return { formatFilters, setHistoryFilters };
})(observer(FilterDialog));
