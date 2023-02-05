import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setObservedDate) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().subtract(10, "year")
  );
};

const onRightClick = (setObservedDate) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().add(10, "year")
  );
};

export const YearsHeader = ({ observedDate, setObservedDate }) => {
  const selectedYear = observedDate.year();
  const firstYear = selectedYear - (selectedYear % 10);

  return (
    <HeaderContainer>
      <Title disabled>
        {firstYear}-{firstYear + 9}
      </Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setObservedDate)}
        onRightClick={() => onRightClick(setObservedDate)}
      />
    </HeaderContainer>
  );
};
