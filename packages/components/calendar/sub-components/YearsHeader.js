import moment from "moment";
import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

export const YearsHeader = ({
  observedDate,
  setObservedDate,
  minDate,
  maxDate,
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
      <Title disabled>
        {firstYear}-{firstYear + 9}
      </Title>
      <HeaderButtons
        onLeftClick={onLeftClick}
        onRightClick={onRightClick}
        isLeftDisabled={isLeftDisabled}
        isRightDisabled={isRightDisabled}
      />
    </HeaderContainer>
  );
};
