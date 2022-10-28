import { convertToParamMap, ParamMap, Params } from "@angular/router";
import { ReplaySubject } from "rxjs";

// reference: https://stackoverflow.com/questions/40789637/how-to-mock-an-activatedroute-parent-route-in-angular2-for-testing-purposes
export class ActivatedRouteStub {
    // Use a ReplaySubject to share previous values with subscribers
    // and pump new values into the `paramMap` observable
    private subject = new ReplaySubject<ParamMap>();

    constructor(initialParams?: Params) {
      this.setParamMap(initialParams);
    }
    /** The mock paramMap observable */
    readonly paramMap = this.subject.asObservable();

    /** Set the paramMap observables's next value */
    setParamMap(params?: Params) {
      this.subject.next(convertToParamMap(params));
    }
}
