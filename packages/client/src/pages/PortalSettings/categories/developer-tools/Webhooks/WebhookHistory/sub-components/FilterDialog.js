import React, { useState } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import styled from "styled-components";

import { Text } from "@docspace/components";
import { SelectorAddButton } from "@docspace/components";
import { SelectedItem } from "@docspace/components";

import { Calendar } from "@docspace/components";

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
  margin-top: 12px;
  margin-bottom: 16px;
`;

const Separator = styled.hr`
  border-top: 1px solid #eceef1;
`;

export const FilterDialog = (props) => {
  const { visible, closeModal, applyFilters } = props;
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);

  const openCalendar = () => setIsCalendarOpen(true);
  const closeCalendar = () => setIsCalendarOpen(false);

  const setDeliveryDate = (date) => {
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryDate: date }));
  };

  const [filterSettings, setFilterSettings] = useState({
    deliveryDate: null,
    deliveryTime: null,
    status: [],
  });

  console.log(filterSettings);

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={closeModal} displayType="aside">
      <ModalDialog.Header>Search options</ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontWeight={600} fontSize="15px">
          Delivery date
        </Text>
        <Selectors>
          <span>
            <SelectorAddButton title="add" onClick={openCalendar} style={{ marginRight: "8px" }} />
            <Text isInline fontWeight={600} color="#A3A9AE">
              Select date
            </Text>
            {isCalendarOpen ? (
              <Calendar
                selectedDate={filterSettings.deliveryDate}
                setSelectedDate={setDeliveryDate}
                onChange={closeCalendar}
              />
            ) : (
              <></>
            )}
          </span>

          {/* <SelectedItem onClose={() => {}} text="Selected item" /> */}
        </Selectors>
        <Separator />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button label="Apply" size="normal" primary={true} onClick={applyFilters} />
          <Button label="Cancel" size="normal" onClick={closeModal} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};
