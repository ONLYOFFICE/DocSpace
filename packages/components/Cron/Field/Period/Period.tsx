import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";

import ComboBox from "../../../combobox";
import { getOptions } from "./Period.helper";

import PeriodProps, { PeriodOptionType } from "./Period.props";

function Period({ period = "Hour", setPeriod }: PeriodProps) {
  const { t } = useTranslation("Cron");

  const onSelect = (arg: PeriodOptionType) => {
    setPeriod(arg.key);
  };

  const options = useMemo(() => getOptions(t), []);
  const selectedOption = useMemo(
    () => ({ key: period, label: t(`Every${period}`) }),
    [period]
  );

  return (
    <ComboBox
      scaledOptions
      size="content"
      scaled={false}
      noBorder={false}
      options={options}
      showDisabledItems
      onSelect={onSelect}
      selectedOption={selectedOption}
    />
  );
}

export default Period;
