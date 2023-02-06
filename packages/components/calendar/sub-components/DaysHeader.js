import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setObservedDate) =>
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().subtract(1, "month")
  );

const onRightClick = (setObservedDate) =>
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().add(1, "month")
  );

export const DaysHeader = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  minDate,
  maxDate,
}) => {
  const isLeftDisabled =
    observedDate.clone().subtract(1, "month").endOf("month") < minDate;
  const isRightDisabled =
    observedDate.clone().add(1, "month").startOf("month") > maxDate;
  return (
    <HeaderContainer>
      <Title
        onClick={() =>
          setSelectedScene((prevSelectedScene) => prevSelectedScene + 1)
        }
      >
        {observedDate.format("MMMM")} {observedDate.year()}
      </Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setObservedDate)}
        onRightClick={() => onRightClick(setObservedDate)}
        isLeftDisabled={isLeftDisabled}
        isRightDisabled={isRightDisabled}
      />
    </HeaderContainer>
  );
};
