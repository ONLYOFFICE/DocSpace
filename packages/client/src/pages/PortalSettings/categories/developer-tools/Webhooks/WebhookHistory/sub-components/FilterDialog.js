import React, { useState } from "react";
import moment from "moment";

import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";

import { Text } from "@docspace/components";
import { SelectorAddButton } from "@docspace/components";
import { SelectedItem } from "@docspace/components";

import { Calendar } from "@docspace/components";
import { Button } from "@docspace/components";

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

const RoundedButton = styled(Button)`
  font-size: 13px;
  font-weight: 400;

  border-radius: 16px;
  margin-right: 8px;
`;

export const FilterDialog = (props) => {
  const { visible, closeModal, applyFilters } = props;
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);

  const toggleCalendar = () => setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);
  const closeCalendar = () => setIsCalendarOpen(false);

  const [filterSettings, setFilterSettings] = useState({
    deliveryDate: null,
    deliveryTime: null,
    status: [],
  });

  const setDeliveryDate = (date) =>
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryDate: date }));

  const deleteSelectedDate = () =>
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryDate: null }));

  const toggleStatus = (e) => {
    const statusName = e.target.textContent;
    if (filterSettings.status.includes(statusName)) {
      setFilterSettings((prevFilterSetting) => ({
        ...prevFilterSetting,
        status: prevFilterSetting.status.filter((statusItem) => statusItem !== statusName),
      }));
    } else {
      setFilterSettings((prevFilterSetting) => ({
        ...prevFilterSetting,
        status: [...prevFilterSetting.status, statusName],
      }));
    }
  };

  const isStatusSelected = (statusName) => {
    return filterSettings.status.includes(statusName);
  };

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={closeModal} displayType="aside">
      <ModalDialog.Header>Search options</ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontWeight={600} fontSize="15px">
          Delivery date
        </Text>
        <Selectors>
          {filterSettings.deliveryDate === null ? (
            <span>
              <SelectorAddButton
                title="add"
                onClick={toggleCalendar}
                style={{ marginRight: "8px" }}
              />
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
          ) : (
            <SelectedItem
              onClose={deleteSelectedDate}
              text={moment(filterSettings.deliveryDate).format("DD MMM YYYY")}
            />
          )}
          {/* {filterSettings.deliveryDate !== null ? (
            <span>
              <SelectorAddButton
                title="add"
                onClick={toggleCalendar}
                style={{ marginRight: "8px" }}
              />
              <Text isInline fontWeight={600} color="#A3A9AE">
                Select Delivery time
              </Text>
            </span>
          ) : (
            <SelectedItem
              onClose={deleteSelectedDate}
              text={moment(filterSettings.deliveryDate).format("DD MMM YYYY")}
            />
          )} */}
        </Selectors>
        <Separator />
        <Text fontWeight={600} fontSize="15px">
          Status
        </Text>
        <Selectors>
          <RoundedButton
            label="Not sent"
            onClick={toggleStatus}
            primary={isStatusSelected("Not sent")}
          />
          <RoundedButton label="2XX" onClick={toggleStatus} primary={isStatusSelected("2XX")} />
          <RoundedButton label="3XX" onClick={toggleStatus} primary={isStatusSelected("3XX")} />
          <RoundedButton label="4XX" onClick={toggleStatus} primary={isStatusSelected("4XX")} />
          <RoundedButton label="5XX" onClick={toggleStatus} primary={isStatusSelected("5XX")} />
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
