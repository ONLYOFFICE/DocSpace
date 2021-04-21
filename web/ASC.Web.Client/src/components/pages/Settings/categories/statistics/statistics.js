import React from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import isEmpty from "lodash/isEmpty";
import styled from "styled-components";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import TabsContainer from "@appserver/components/tabs-container";
import ProgressBar from "@appserver/components/progress-bar";
import toastr from "@appserver/components/toast/toastr";
import Chart from "@appserver/components/chart";
import DatePicker from "@appserver/components/date-picker";
import { tablet, mobile } from "@appserver/components/utils/device";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import ArrowRightIcon from "@appserver/studio/public/images/arrow.right.react.svg";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const MainContainer = styled.div`
  width: 100%;

  .settings_tabs {
    padding-bottom: 16px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }

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

    .link-text {
      margin: 0;
    }

    .visits-chart {
      height: 230px;
    }

    .visits-datepicker-container {
      .visits-datepicker {
        display: inline-block;
        margin-left: 4px;
        margin-right: 4px;
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
      visitsData: this.getTestData(),
    };
  }

  async componentDidMount() {
    const { quota, visits, getQuota, getVisits } = this.props;

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
        await getVisits(
          new Date("2021/01/13").toISOString(),
          new Date().toISOString()
        );
      } catch (error) {
        toastr.error(error);
      }
    }

    hideLoader();
  }

  /* 
  https://dotnet.onlyoffice.com:8093/api/2.0/settings/quota
  "response": {
"storageSize": 10995116277760,
"maxFileSize": 107374182400,
"usedSize": 1596396544,
"maxUsersCount": 10000,
"usersCount": 1058,
"availableSize": 10993519881216,
"availableUsersCount": 8942,
"storageUsage": [
{
"path": "files",
"size": 1596396544
}
],
"userStorageSize": 0,
"userUsedSize": 0,
"userAvailableSize": 0
},
  */

  getRandData = (items) => {
    return items.map(
      () => Math.floor(Math.random() * (Math.floor(100) - 0)) + 0
    );
  };

  getTestData = (period = 7, title = "Data ") => {
    const labels = Array.from({ length: period }, (v, i) => ++i);

    return {
      labels: labels,
      datasets: [
        {
          label: "Visits",
          data: this.getRandData(labels),
        },
      ],
    };
  };

  toUserValue = (value) => {
    const sizes = ["B", "Kb", "Mb", "Gb", "Tb"];
    if (value == 0) return "0 B";
    const i = parseInt(Math.floor(Math.log(value) / Math.log(1024)));
    return Math.round(value / Math.pow(1024, i), 2) + " " + sizes[i];
  };

  setPeriod = (period) => {
    if (period.value) period = period.value;
    this.setState({
      visitsData: this.getTestData(period),
    });
  };

  render() {
    const { t, quota, visits } = this.props;
    const { maxUsersCount, usersCount, storageSize, usedSize } = quota;
    const { visitsData } = this.state;

    const convertedData = {
      labels: visits.map((item) => item.displayDate),
      datasets: [
        {
          label: "Посещения",
          data: visits.map((item) => item.hits),
        },
      ],
    };

    console.log(convertedData);

    const storageSizeConverted = this.toUserValue(storageSize);
    const usedSizeConverted = this.toUserValue(usedSize);
    const storageUsedPercent = parseFloat(
      ((usedSize * 100) / storageSize).toFixed(2)
    );

    const visitTabs = [
      {
        key: "week",
        title: t("VisitsWeek"),
        value: 7,
        content: "",
      },
      {
        key: "month",
        title: t("VisitsMonth"),
        value: 30,
        content: "",
      },
      {
        key: "threeMonth",
        title: t("VisitsThreeMonth"),
        value: 90,
        content: "",
      },
      {
        key: "period",
        title: t("VisitsPeriod"),
        content: "",
        isDisabled: true,
      },
    ];

    return (
      <MainContainer>
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              className="inherit-title-link header"
              truncate={true}
              href="#"
            >
              {t("StorageTitle")}
            </Link>
            <StyledArrowRightIcon size="small" color="#333333" />
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
          <TabsContainer onSelect={this.setPeriod} elements={visitTabs} />
          <div className="visits-datepicker-container">
            <DatePicker
              className="visits-datepicker"
              onChange={(date) => {
                console.log("Selected date", date);
              }}
              selectedDate={new Date()}
              minDate={new Date("1970/01/01")}
              maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
              isDisabled={true}
              isReadOnly={false}
              hasError={false}
              isOpen={false}
              themeColor="#ED7309"
              locale="en"
            />
            {`-`}
            <DatePicker
              className="visits-datepicker"
              onChange={(date) => {
                console.log("Selected date", date);
              }}
              selectedDate={new Date()}
              minDate={new Date("1970/01/01")}
              maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
              isDisabled={true}
              isReadOnly={false}
              hasError={false}
              isOpen={false}
              themeColor="#ED7309"
              locale="en"
            />
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
