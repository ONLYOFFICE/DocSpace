import { CurrentDateItem, DateItem, SecondaryDateItem, YearsContainer } from "../styled-components"

export const YearsBody = () => {
  const years = [2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034];

  const yearsElements = years.map(year => <SecondaryDateItem big>{year}</SecondaryDateItem>);

  for(let i = 1; i < 11; i++){
    yearsElements[i] = <DateItem big>{years[i]}</DateItem>;
  }

  yearsElements[3] = <CurrentDateItem big>{years[3]}</CurrentDateItem>

  return <YearsContainer>{yearsElements}</YearsContainer>;
};
