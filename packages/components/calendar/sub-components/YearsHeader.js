import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setSelectedDate) => {
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().subtract(10, "year")
  );
};

const onRightClick = (setSelectedDate) => {
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().add(10, "year")
  );
};

export const YearsHeader = ({ selectedDate, setSelectedDate }) => {
  const selectedYear = selectedDate.year();
  const firstYear = selectedYear - (selectedYear % 10);

  return (
    <HeaderContainer>
      <Title disabled>
        {firstYear}-{firstYear + 9}
      </Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setSelectedDate)}
        onRightClick={() => onRightClick(setSelectedDate)}
      />
    </HeaderContainer>
  );
};
