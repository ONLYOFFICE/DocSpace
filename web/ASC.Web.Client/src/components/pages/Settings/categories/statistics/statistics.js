import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import moment from "moment";
import isEmpty from "lodash/isEmpty";
import styled from "styled-components";
import Text from "@appserver/components/text";
import TabsContainer from "@appserver/components/tabs-container";
import ProgressBar from "@appserver/components/progress-bar";
import toastr from "@appserver/components/toast/toastr";
import Chart from "@appserver/components/chart";
import DatePicker from "@appserver/components/date-picker";
import ComboBox from "@appserver/components/combobox";
import Loader from "@appserver/components/loader";
import { isMobile, isTablet, mobile } from "@appserver/components/utils/device";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { setDocumentTitle } from "../../../../../helpers/utils";

const MainContainer = styled.div`
  width: 100%;

  .category-item-wrapper {
    margin-bottom: 40px;

    .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: #555f65;
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    }

    .visits-chart {
      height: 230px;
    }

    .visits-selectors-container {
      display: inline-flex;

      @media ${mobile} {
        display: block;
      }

      .scrollbar {
        width: 340px !important;
      }

      .visits-datepicker-container {
        @media ${mobile} {
          display: flex;
          align-items: baseline;
        }

        .visits-datepicker {
          display: inline-block;
          margin-left: 4px;
          margin-right: 4px;
          margin-bottom: 8px;

          @media ${mobile} {
            display: inline-flex;
            width: auto;
            margin-top: 8px;

            :first-of-type {
              margin-left: 0px;
            }

            :last-of-type {
              margin-right: 0px;
            }
          }
        }
      }
    }

    .storage-value-title {
      max-width: 420px;

      .storage-value-current {
        font-family: Open Sans;
        font-size: 13px;
        font-style: normal;
        font-weight: 600;
        line-height: 18px;
        letter-spacing: 0px;
        text-align: left;
        display: inline-block;
      }

      .storage-value-max {
        font-family: Open Sans;
        font-size: 13px;
        font-style: normal;
        font-weight: 600;
        line-height: 18px;
        letter-spacing: 0px;
        text-align: left;
        margin-left: auto;
        float: right;
      }
    }
  }
`;

const Storage = ({ quota, t }) => {
  const { storageSize, usedSize } = quota;
  const toUserValue = (value) => {
    const sizes = ["B", "Kb", "Mb", "Gb", "Tb"];
    if (value == 0) return "0 B";
    const i = parseInt(Math.floor(Math.log(value) / Math.log(1024)));
    return Math.round(value / Math.pow(1024, i), 2) + " " + sizes[i];
  };

  const storageSizeConverted = toUserValue(storageSize);
  const usedSizeConverted = toUserValue(usedSize);
  const storageUsedPercent = parseFloat(
    ((usedSize * 100) / storageSize).toFixed(2)
  );

  return (
    <div className="category-item-wrapper">
      <div className="category-item-heading">
        <Text className="inherit-title-link header" truncate={true}>
          {t("StorageTitle")}
        </Text>
      </div>
      <div className="storage-value-title">
        <div className="storage-value-current">{usedSizeConverted}</div>
        <div className="storage-value-max">{storageSizeConverted}</div>
        <ProgressBar percent={storageUsedPercent} />
      </div>
      <Text className="category-item-description">{t("StorageInfo")}</Text>
    </div>
  );
};

const Users = ({ quota, t }) => {
  const { maxUsersCount, usersCount } = quota;

  return (
    <Text className="category-item-subheader">
      {t("ActiveUsers", {
        current: usersCount,
        total: maxUsersCount,
      })}
    </Text>
  );
};

const VisitsSelectors = ({ getVisits, i18n, t }) => {
  useEffect(() => {
    (async () => {
      showLoader();

      try {
        await getVisitsData("week");
      } catch (error) {
        toastr.error(error);
      }

      hideLoader();
    })();
  }, []);

  const [isPeriod, setIsPeriod] = useState(false);
  const [selectedPeriod, setSelectedPeriod] = useState(null);
  const [visitsDateFrom, setDateFrom] = useState(
    moment().subtract(6, "months").toDate()
  );
  const [visitsDateTo, setDateTo] = useState(moment().toDate());

  const visitTabs = [
    {
      key: "week",
      title: t("VisitsWeek"),
      label: t("VisitsWeek"),
    },
    {
      key: "month",
      title: t("VisitsMonth"),
      label: t("VisitsMonth"),
    },
    {
      key: "threeMonth",
      title: t("VisitsThreeMonth"),
      label: t("VisitsThreeMonth"),
    },
    {
      key: "period",
      title: t("VisitsPeriod"),
      label: t("VisitsPeriod"),
    },
  ];

  const getVisitsData = (period) => {
    switch (period) {
      case "week":
        return getVisits(
          moment().subtract(7, "days").utc().format(),
          moment().utc().format()
        );
      case "month":
        return getVisits(
          moment().subtract(1, "months").utc().format(),
          moment().utc().format()
        );
      case "threeMonth":
        return getVisits(
          moment().subtract(3, "months").utc().format(),
          moment().utc().format()
        );
      case "period":
        return getVisits(
          moment(visitsDateFrom).utc().format(),
          moment(visitsDateTo).utc().format()
        );
    }
  };

  const setPeriodFromDate = (date) => {
    setDateFrom(date).then(() => getVisitsData("period"));
  };

  const setPeriodToDate = (date) => {
    setDateTo(date).then(() => getVisitsData("period"));
  };

  const setPeriodData = (period) => {
    const { key } = period;

    setIsPeriod(key === "period");
    setSelectedPeriod(period);

    return getVisitsData(key);
  };

  return (
    <div className="visits-selectors-container">
      {window.innerWidth < 1024 ? (
        <ComboBox
          options={visitTabs}
          onSelect={setPeriodData}
          selectedOption={selectedPeriod || visitTabs[0]}
          scaledOptions={true}
          scaled={!tablet}
          noBorder={false}
          isDisabled={false}
          showDisabledItems={true}
        />
      ) : (
        <TabsContainer onSelect={setPeriodData} elements={visitTabs} />
      )}
      <div className="visits-datepicker-container">
        <DatePicker
          className="visits-datepicker"
          onChange={setPeriodFromDate}
          selectedDate={visitsDateFrom}
          maxDate={moment().subtract(1, "days").toDate()}
          isDisabled={!isPeriod}
          locale={i18n.language}
        />
        {`-`}
        <DatePicker
          className="visits-datepicker"
          onChange={setPeriodToDate}
          selectedDate={visitsDateTo}
          maxDate={moment().toDate()}
          minDate={moment(visitsDateFrom).add(1, "days").toDate()}
          isDisabled={!isPeriod}
          locale={i18n.language}
        />
      </div>
    </div>
  );
};

const Statistics = ({ ...props }) => {
  const { t, tReady, i18n, quota, visits, getQuota, getVisits } = props;

  useEffect(() => {
    setDocumentTitle(t("ManagementCategoryStatistic"));
  }, [setDocumentTitle]);

  useEffect(() => {
    (async () => {
      showLoader();

      if (isEmpty(quota, true)) {
        try {
          await getQuota();
        } catch (error) {
          toastr.error(error);
        }
      }

      hideLoader();
    })();
  }, []);

  const chartData = {
    labels: visits.map((item) => moment(item.date).format("D MMM")),
    datasets: [
      {
        label: t("Visits"),
        data: visits.map((item) => item.hits),
      },
    ],
  };

  return tReady ? (
    <MainContainer className="not-selectable">
      <Storage quota={quota} t={t} />
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Text className="inherit-title-link header" truncate={true}>
            {t("VisitsGraph")}
          </Text>
        </div>
        <Users quota={quota} t={t} />
        <VisitsSelectors t={t} i18n={i18n} getVisits={getVisits} />
        <Chart className="visits-chart" data={chartData} />
      </div>
    </MainContainer>
  ) : (
    <Loader className="pageLoader" type="rombs" size="40px" />
  );
};

Statistics.propTypes = {
  t: PropTypes.func,
  i18n: PropTypes.object,
};

export default inject(({ setup }) => {
  const { getQuota, getVisits } = setup;
  const { quota, visits } = setup.statistic;

  return {
    quota,
    visits,
    getQuota,
    getVisits,
  };
})(withTranslation("Settings")(observer(Statistics)));
