import { initReactI18next } from "react-i18next";
import HttpBackend from "i18next-http-backend";
import i18n from "i18next";

const newInstance = i18n.createInstance();

newInstance
  .use(HttpBackend)
  .use(initReactI18next)
  .init({
    backend: {
      backendOptions: [
        {
          loadPath: "../../client/public/locales/{{lng}}/{{ns}}.json",
        },
        {
          loadPath: "../../../public/locales/{{lng}}/{{ns}}.json",
        },
      ],
    },
    // resources: {
    //   en: {
    //     Common: {
    //       EveryYear: "Every year",
    //       EveryMonth: "Every month",
    //       EveryDay: "Every day",
    //       EveryWeek: "Every week",
    //       EveryHour: "Every hour",
    //       EveryMinute: "Every minute",
    //       In: "in",
    //       At: "at",
    //       On: "on",
    //       And: "and",
    //       EveryDayOfTheMonth: "Every day of the month",
    //       DayOfTheMonth: "Day of the month",
    //       EveryDayOfTheWeek: "Every day of the week",
    //       DayOfTheWeek: "Day of the week",

    //       JAN: "JAN",
    //       FEB: "FEB",
    //       MAR: "MAR",
    //       APR: "APR",
    //       MAY: "MAY",
    //       JUN: "JUN",
    //       JUL: "JUL",
    //       AUG: "AUG",
    //       SEP: "SEP",
    //       OCT: "OCT",
    //       NOV: "NOV",
    //       DEC: "DEC",

    //       SUN: "SUN",
    //       MON: "MON",
    //       TUE: "TUE",
    //       WED: "WED",
    //       THU: "THU",
    //       FRI: "FRI",
    //       SAT: "SAT",
    //     },
    //   },
    //   ru: {
    //     Common: {
    //       EveryYear: "Каждый год",
    //       EveryMonth: "Каждый месяц",
    //       EveryDay: "Каждый день",
    //       EveryWeek: "Каждую неделю",
    //       EveryHour: "Каждый час",
    //       EveryMinute: "Каждую минуту",
    //       In: "В",
    //       At: "В",
    //       On: "На",
    //       And: "и",
    //       EveryDayOfTheMonth: "Каждый день месяца",
    //       DayOfTheMonth: "Day of the month",
    //       EveryDayOfTheWeek: "Каждый день недели",
    //       DayOfTheWeek: "Day of the week",

    //       JAN: "Январь",
    //       FEB: "Февраль",
    //       MAR: "Март",
    //       APR: "Апрель",
    //       MAY: "Май",
    //       JUN: "Июнь",
    //       JUL: "Июль",
    //       AUG: "Август",
    //       SEP: "Сентябрь",
    //       OCT: "Октябрь",
    //       NOV: "Ноябрь",
    //       DEC: "Декабрь",

    //       SUN: "Воскресенье",
    //       MON: "Общие",
    //       TUE: "Вторник",
    //       WED: "Среда",
    //       THU: "Четверг",
    //       FRI: "Пятница",
    //       SAT: "Суббота",
    //     },
    //   },
    // },

    lng: "en",
    fallbackLng: "en",
    interpolation: {
      escapeValue: false,
    },
  });

export default newInstance;
