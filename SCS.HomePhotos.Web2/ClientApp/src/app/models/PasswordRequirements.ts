export class PasswordRequirements {

    constructor() {
        this.minLength = 8;
        this.uppercaseCharacters = 1;
        this.digits = 1;
        this.specialCharacters = 1;
    }
    minLength?: number;
    uppercaseCharacters?: number;
    digits?: number;
    specialCharacters?: number;
}
