import React, { useState } from "react";
import { inject, observer } from "mobx-react";

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
`;

const Separator = styled.hr`
  border-top: 1px solid #eceef1;
  margin-bottom: 16px;
`;

const FilterDialog = (props) => {
  const { visible, closeModal, applyFilters, filterSettings, formatFilters, setStatusFilters } =
    props;

  const [isApplied, setIsApplied] = useState(false);

  const handleApplyFilters = () => {
    if (filterSettings.deliveryTo > filterSettings.deliveryFrom) {
      const params = formatFilters(filterSettings);

      setStatusFilters(filterSettings);
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
        />
        <Separator />
        <StatusPicker Selectors={Selectors} />
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
  const { filterSettings, formatFilters } = webhooksStore;

  return { filterSettings, formatFilters };
})(observer(FilterDialog));
