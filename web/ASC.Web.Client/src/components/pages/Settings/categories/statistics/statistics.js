import React from "react";
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
import { isMobile, isTablet } from "react-device-detect";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";

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
      display: ${isMobile && !isTablet ? "block" : "inline-flex"};

      .scrollbar {
        width: 340px !important;
      }

      .visits-datepicker-container {
        ${isMobile &&
        !isTablet &&
        `
        display: flex; 
        align-items: baseline;
        `}

        .visits-datepicker {
          display: inline-block;
          margin-left: 4px;
          margin-right: 4px;
          margin-bottom: 8px;
          ${isMobile &&
          !isTablet &&
          `
          display: inline-flex;
          width: auto;
          margin-top: 8px;

          :first-of-type {
            margin-left: 0px;
          }

          :last-of-type {
            margin-right: 0px;
          }
          `};
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

class Statistics extends React.Component {
  constructor(props) {
    super(props);
    const { t } = props;
    document.title = `${t("ManagementCategoryStatistic")}`;

    this.state = {
      isLoading: false,
      isPeriod: false,
      visitsDateFrom: moment().subtract(6, "months").toDate(),
      visitsDateTo: moment().toDate(),
      selectedPeriod: null,
    };
  }

  async componentDidMount() {
    const { quota, visits, getQuota } = this.props;

    showLoader();
    if (isEmpty(quota, true)) {
      try {
        await getQuota();
      } catch (error) {
        toastr.error(error);
      }
    }

    if (isEmpty(visits, true)) {
      try {
        await this.getVisitsData("week");
      } catch (error) {
        toastr.error(error);
      }
    }

    hideLoader();
  }

  getVisitsData = (period) => {
    const { getVisits } = this.props;
    const { visitsDateFrom, visitsDateTo } = this.state;
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

  setPeriodData = (period) => {
    const { key } = period;

    if (key === "period") {
      this.setState({
        isPeriod: true,
        selectedPeriod: period,
      });
    } else {
      this.setState({
        isPeriod: false,
        selectedPeriod: period,
      });
    }

    return this.getVisitsData(key);
  };

  setPeriodFromDate = (date) => {
    this.setState(
      {
        visitsDateFrom: date,
      },
      () => this.getVisitsData("period")
    );
  };

  setPeriodToDate = (date) => {
    this.setState(
      {
        visitsDateTo: date,
      },
      () => this.getVisitsData("period")
    );
  };

  toUserValue = (value) => {
    const sizes = ["B", "Kb", "Mb", "Gb", "Tb"];
    if (value == 0) return "0 B";
    const i = parseInt(Math.floor(Math.log(value) / Math.log(1024)));
    return Math.round(value / Math.pow(1024, i), 2) + " " + sizes[i];
  };

  render() {
    const { t, quota, visits, culture } = this.props;
    const { maxUsersCount, usersCount, storageSize, usedSize } = quota;
    const {
      isPeriod,
      visitsDateFrom,
      visitsDateTo,
      selectedPeriod,
    } = this.state;

    const convertedData = {
      labels: visits.map((item) => moment(item.date).format("D MMM")),
      datasets: [
        {
          label: t("Visits"),
          data: visits.map((item) => item.hits),
        },
      ],
    };

    const storageSizeConverted = this.toUserValue(storageSize);
    const usedSizeConverted = this.toUserValue(usedSize);
    const storageUsedPercent = parseFloat(
      ((usedSize * 100) / storageSize).toFixed(2)
    );

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

    return (
      <MainContainer className="not-selectable">
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
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Text className="inherit-title-link header" truncate={true}>
              {t("VisitsGraph")}
            </Text>
          </div>
          <Text className="category-item-subheader">
            {t("ActiveUsers", {
              current: usersCount,
              total: maxUsersCount,
            })}
          </Text>
          <div className="visits-selectors-container">
            {isMobile ? (
              <ComboBox
                options={visitTabs}
                onSelect={this.setPeriodData}
                selectedOption={selectedPeriod || visitTabs[0]}
                scaledOptions={true}
                scaled={!isTablet}
                noBorder={false}
                isDisabled={false}
                showDisabledItems={true}
              />
            ) : (
              <TabsContainer
                onSelect={this.setPeriodData}
                elements={visitTabs}
              />
            )}
            <div className="visits-datepicker-container">
              <DatePicker
                className="visits-datepicker"
                onChange={this.setPeriodFromDate}
                selectedDate={visitsDateFrom}
                maxDate={moment().subtract(1, "days").toDate()}
                isDisabled={!isPeriod}
                locale={culture}
              />
              {`-`}
              <DatePicker
                className="visits-datepicker"
                onChange={this.setPeriodToDate}
                selectedDate={visitsDateTo}
                maxDate={moment().toDate()}
                minDate={moment(visitsDateFrom).add(1, "days").toDate()}
                isDisabled={!isPeriod}
                locale={culture}
              />
            </div>
          </div>
          <Chart className="visits-chart" data={convertedData} />
        </div>
      </MainContainer>
    );
  }
}

Statistics.propTypes = {
  t: PropTypes.func,
  i18n: PropTypes.object,
};

export default inject(({ auth, setup }) => {
  const { getQuota, getVisits } = setup;
  const { quota, visits } = setup.statistic;
  const { culture } = auth.settingsStore;

  return {
    quota,
    visits,
    culture,
    getQuota,
    getVisits,
  };
})(withTranslation("Settings")(observer(Statistics)));
