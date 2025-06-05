plugins {
    kotlin("jvm") version "2.1.10"
    application
}

application {
    mainClass.set("MainKt")
}

dependencies {
    implementation("com.github.TeamNewPipe:NewPipeExtractor:0.24.6")
    implementation("io.ktor:ktor-client-core:3.1.3")
    implementation("io.ktor:ktor-client-cio:3.1.3")
}

repositories {
    mavenCentral()
    maven {
        url = uri("https://jitpack.io")
    }
}
