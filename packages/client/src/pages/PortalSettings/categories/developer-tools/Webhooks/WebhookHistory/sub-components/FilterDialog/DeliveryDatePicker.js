import React from "react";

import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import DateTimePicker from "@docspace/components/date-time-picker";

const DeliveryDatePicker = ({ filters, setFilters, isApplied, setIsApplied }) => {
  const { t } = useTranslation(["Webhooks"]);

  const onChange = (dateObj) => {
    setFilters((prevFilters) => ({
      deliveryDate: dateObj.date,
      deliveryFrom: dateObj.timeFrom,
      deliveryTo: dateObj.timeTo,
      status: prevFilters.status,
    }));
  };

  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        {t("DeliveryDate")}
      </Text>
      <DateTimePicker
        isApplied={isApplied}
        setIsApplied={setIsApplied}
        onChange={onChange}
        initialDate={filters.deliveryDate}
        initialTimeFrom={filters.deliveryFrom}
        initialTimeTo={filters.deliveryTo}
        selectDateText={t("SelectDate")}
        fromText={t("From")}
        beforeText={t("Before")}
        selectTimeText={t("SelectDeliveryTime")}
      />
    </>
  );
};

export default DeliveryDatePicker;
