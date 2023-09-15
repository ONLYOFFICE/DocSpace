import React from "react";
import moment from "moment";
import { HeaderActionIcon, HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

export const YearsHeader = ({
  observedDate,
  setObservedDate,
  minDate,
  maxDate,
  isMobile,
}) => {
  const selectedYear = observedDate.year();
  const firstYear = selectedYear - (selectedYear % 10);

  const onLeftClick = () =>
    setObservedDate((prevObservedDate) =>
      prevObservedDate.clone().subtract(10, "year")
    );

  const onRightClick = () =>
    setObservedDate((prevObservedDate) =>
      prevObservedDate.clone().add(10, "year")
    );

  const isLeftDisabled =
    moment(`${firstYear - 1}`)
      .endOf("year")
      .endOf("month") < minDate;
  const isRightDisabled = moment(`${firstYear + 10}`) > maxDate;

  return (
    <HeaderContainer>
      <Title disabled className="years-header" isMobile={isMobile}>
        {moment(firstYear, "YYYY").format("YYYY")}-
        {moment(firstYear + 9, "YYYY").format("YYYY")}
        <HeaderActionIcon isMobile={isMobile} />
      </Title>
      <HeaderButtons
        onLeftClick={onLeftClick}
        onRightClick={onRightClick}
        isLeftDisabled={isLeftDisabled}
        isRightDisabled={isRightDisabled}
        isMobile={isMobile}
      />
    </HeaderContainer>
  );
};
